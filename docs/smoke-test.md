# Production Smoke Test

Run after every deploy. No frontend needed — PowerShell only.

**Prerequisite:** after any password reset, first pass the `stored-verify = True`
gate in `password-reset.md`. Step 2 below returns `401 Invalid credentials.` if the
database holds a wrong or stale hash — that is a data problem, not an app problem.

```powershell
$base = "https://dvld.tryasp.net"

# 1. Liveness — expect "Healthy" (HTTP 200)
Invoke-RestMethod "$base/health"

# 2. Login — expect token + refreshToken (HTTP 200)
$login = Invoke-RestMethod "$base/api/auth/login" -Method Post `
  -ContentType "application/json" `
  -Body '{"userName":"<admin-user>","password":"<admin-password>"}'
$headers = @{ Authorization = "Bearer $($login.token)" }

# 3. Authenticated paged list — expect items + totalCount (HTTP 200)
Invoke-RestMethod "$base/api/people?PageNumber=1&PageSize=5" -Headers $headers

# 4. Refresh rotation — expect a NEW token pair (HTTP 200)
$ref = Invoke-RestMethod "$base/api/auth/refresh" -Method Post `
  -ContentType "application/json" `
  -Body (@{ token = $login.refreshToken } | ConvertTo-Json)

# 5. Old refresh token must now be dead — expect HTTP 401
try {
  Invoke-RestMethod "$base/api/auth/refresh" -Method Post `
    -ContentType "application/json" `
    -Body (@{ token = $login.refreshToken } | ConvertTo-Json)
  "FAIL: old refresh token still accepted"
} catch { "OK: old refresh token rejected ($($_.Exception.Response.StatusCode))" }

# 6. Wrong password — expect HTTP 401, no details leaked
try {
  Invoke-RestMethod "$base/api/auth/login" -Method Post `
    -ContentType "application/json" `
    -Body '{"userName":"<admin-user>","password":"wrong"}'
} catch { "OK: $($_.Exception.Response.StatusCode)" }

# 7. Missing entity — expect HTTP 404 with ProblemDetails
try {
  Invoke-RestMethod "$base/api/people/2147483647" -Headers $headers
} catch { "OK: $($_.Exception.Response.StatusCode)" }

# 8. Logout — revokes the rotated refresh token (HTTP 200)
Invoke-RestMethod "$base/api/auth/logout" -Method Post `
  -ContentType "application/json" `
  -Body (@{ refreshToken = $ref.refreshToken } | ConvertTo-Json)
```

## Optional — other accounts

```powershell
$b2 = '{"userName":"officer","password":"<officer-password>"}'
Invoke-RestMethod "$base/api/auth/login" -Method Post -ContentType "application/json" -Body $b2
# same pattern for viewer / <viewer-password>
```

## Rate-limit spot check (run last)

Wait ~60 seconds after step 6 (and any optional logins) first — steps 2/4/5/6
already consume the `Auth` budget (5/min/IP), so without the wait the 429 fires
early and misleads. Then fire 6 rapid logins — the 6th must return **429**
(`Auth` policy). If *every* request 429s from different client IPs,
`ForwardedHeaders` is misconfigured (all clients share the proxy IP bucket).

## Troubleshooting

- `400 ... 'u' is an invalid start of a property name` → the inner double quotes
  were stripped while pasting the JSON body. Type that line manually (or build
  `$body = '{"userName":...}'`, echo `$body` to confirm the quotes survived, then
  send `-Body $body`).
- Step 2 `401 Invalid credentials.` → run the `password-reset.md` gate first;
  don't start infrastructure debugging until `stored-verify = True` is proven.

## Swagger

Swagger UI in Production is **env-var gated**: it only appears when the site has
`Swagger__Enabled=true` (MonsterASP panel → environment variables → restart the
site). When enabled: `https://dvld.tryasp.net/swagger` shows the demo-account
instructions. Remove the variable to turn it off again — no redeploy needed.
