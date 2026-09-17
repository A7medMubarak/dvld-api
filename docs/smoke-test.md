# Production Smoke Test

Run after every deploy. No frontend needed — PowerShell only.
Replace `https://api.yourdomain.com` with your production URL.

```powershell
$base = "https://api.yourdomain.com"

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

## Rate-limit spot check

Fire 6 rapid logins — the 6th must return **429** (`Auth` policy: 5 req/min per IP).
If *every* request 429s from different client IPs, `ForwardedHeaders` is misconfigured
(all clients share the proxy IP bucket).

## Swagger

Swagger UI is intentionally **disabled in Production** (`Program.cs` gates it to
Development). Test with the steps above or Postman instead.
