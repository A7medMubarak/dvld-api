# Production Deployment (MonsterASP.net + Plesk)

Runbook for deploying the DVLD API to production. After deploying, verify with
[`smoke-test.md`](smoke-test.md).

## 1. Environment variables (Plesk → your app → Environment Variables)

Never put secrets in JSON or git. Set these in the hosting panel:

| Variable | Value | Notes |
|---|---|---|
| `ASPNETCORE_ENVIRONMENT` | `Production` | Production mode (dev-only diagnostics off) |
| `JwtSettings__SecretKey` | fresh 256-bit key (32+ chars) | Generate per environment, rotate on exposure |
| `ConnectionStrings__DefaultConnection` | `Server=...;initial catalog=DVLD-EFCore;User Id=...;Password=...;TrustServerCertificate=True` | Shared hosts use SQL auth, not `integrated security` |
| `Cors__AllowedOrigins__0` | `https://your-frontend-domain` | Add `__1`, `__2`… for more origins |
| `Swagger__Enabled` | `true` | Enables the public Swagger demo (`/swagger`); remove the variable and restart to turn it off |

Startup fails fast if the secret is missing/short or Issuer/Audience are empty.

## 2. Database (run once from your machine)

```powershell
# Point ONLY this terminal at prod (session-scoped — never machine-wide,
# or local dev will hit production). Clear it when done.
$env:ConnectionStrings__DefaultConnection = "Server=<prod-server>;initial catalog=DVLD-EFCore;User Id=<sql-user>;Password=<sql-password>;TrustServerCertificate=True"

# 1. Create all tables (4 migrations). Requires dotnet-ef 8.x:
#    dotnet new tool-manifest; dotnet tool install dotnet-ef --version 8.0.14
dotnet ef database update --project src\DVLD.DataAccess --startup-project src\DVLD.Api

# 2. Insert starter data — RUN ONCE (re-runs fail on duplicate keys by design)
sqlcmd -S <prod-server> -U <sql-user> -P "<sql-password>" -d DVLD-EFCore -i database\seed.sql

Remove-Item Env:\ConnectionStrings__DefaultConnection
```

Verify 4 rows in the prod DB's `__EFMigrationsHistory` table afterwards.
If `sqlcmd` times out, enable remote SQL access / whitelist your IP in the hosting
panel first. Explicit step — the app never auto-migrates on startup.

## 3. Publish

Pushing to `main` auto-deploys: GitHub Actions (`.github/workflows/deploy.yml`)
builds, tests, and publishes via Web Deploy with the app taken offline.

Manual alternative: in Visual Studio, right-click `DVLD.Api` → **Publish** →
import the host's `.publishsettings` file → **Publish** (Release). This compiles
and uploads in one step — no manual file copying (source code, `tests/`, and
`*.sln` never go to the server).

Publishing moves files only — it does **not** set environment variables, migrate,
or seed. Then in the hosting panel: add your domain, enable the free Let's Encrypt
certificate, and point the uptime monitor at `GET /health`.

## 4. Verify

Follow [`smoke-test.md`](smoke-test.md) — health, login, refresh rotation,
paged list, 401/404 shapes, and the rate-limit spot check. No frontend required.
