# DVLD API

**Driver and Vehicle Licensing Department** — a RESTful backend API built with .NET 8 that manages the full lifecycle of driver licensing: applications, test appointments, license issuance, detention, renewal, and international licensing.

Built for a **government licensing authority**, this system models real-world regulations and business rules — not generic CRUD.

---

## What This Project Demonstrates

### 1. Clean Architecture & Separation of Concerns

The solution is split into **4 projects** with strict dependency rules:

```
DVLD.Contracts   ← DTOs, enums, interfaces, validators (no dependencies)
DVLD.DataAccess  ← EF Core, repositories, mappings (depends only on Contracts)
DVLD.Business    ← Services, business logic, guards (depends only on Contracts)
DVLD.Api         ← Controllers, middleware, DI config (depends on all above)
```

The controller never touches `DbContext`. Database schema changes don't leak into the API layer. Each layer is independently testable — exactly how production .NET teams structure their code.

### 2. RESTful API Design — 14 Controllers, One Convention

Every controller follows the same consistent pattern:

| Aspect | Convention | Example |
|---|---|---|
| Routes | `api/<plural-kebab>` | `api/local-driving-license-applications` |
| GET by ID | → 200 with DTO, 404 if missing | Every controller |
| GET list | → 200 with paginated result | Every controller |
| POST | → 201 with Location header | 12 controllers |
| PATCH status | → 204 No Content | `ApplicationsController` |
| DELETE | → 204 No Content | 10 controllers |

Consistent API patterns mean lower maintenance cost, predictable error handling, and reliable contracts for frontend teams.

### 3. Authentication & Authorization — JWT with Refresh Token Rotation

```
POST /api/auth/login      → { accessToken, refreshToken }
POST /api/auth/refresh    → { newAccessToken, newRefreshToken }
POST /api/auth/logout     → revokes refresh token
```

- Passwords: **BCrypt** (work factor 11) — never stored in plaintext
- Refresh tokens: **SHA-256 hashed** before storage — raw tokens never persisted
- Access control: `[Authorize(Roles = "Admin")]`, `[Authorize(Roles = "Officer")]`, `[AllowAnonymous]`
- Token rotation: Each refresh invalidates the previous token (replay attack prevention)

### 4. Input Validation — FluentValidation + AutoValidation

Every request is automatically validated before reaching the controller — 20+ validators, zero manual wiring:

```csharp
// Single line registers ALL validators in the assembly:
services.AddValidatorsFromAssemblyContaining<LicenseClassWriteRequest>();
```

Invalid requests never hit business logic. Validators are decoupled, reusable, and testable in isolation.

### 5. Domain Expertise — Government Licensing Workflows

This system encodes actual government licensing regulations:

- **Applications**: New, renewal, replacement, international permit, license release
- **Test pipeline**: Vision → Written → Street, with pass/fail gates between stages
- **License lifecycle**: Issue → Active → Detained → Released / Expired → Renewed
- **International permits**: Issued against a valid local license
- **Audit trail**: Every action tracked with `CreatedByUserId` and timestamps
- **Business rules**: A person can hold only one active license per class, detained licenses cannot be renewed, test results determine eligibility for next stage

These workflows aren't generic CRUD — they required understanding a complex domain and translating real regulations into testable, maintainable code.

### 6. Global Exception Handling — Consistent Error Responses

Every exception maps to a standard `ProblemDetails` response:

```csharp
ArgumentException        → 400 Bad Request
KeyNotFoundException     → 404 Not Found
ResourceConflictException → 409 Conflict
UnauthorizedAccessException → 401 Unauthorized
_                        → 500 Internal Server Error
```

**500 errors never leak details to the client** — the real exception is logged server-side with full stack trace, and the client receives only "An unexpected error occurred. Please try again later."

### 7. Rate Limiting — 3 Protection Policies

| Policy | Limit | Applies To |
|---|---|---|
| Global | 100 req/min per authenticated user | All `[Authorize]` endpoints |
| Auth | 5 req/min per IP | Login, refresh |
| Sensitive | 10 req/min per user | Password change |

Anonymous requests to authenticated endpoints consume a separate anonymous limit. This is brute-force mitigation straight out of .NET 8's built-in rate limiter.

### 8. Structured Logging — 3 Streams, Zero Blind Spots

```
logs/dvld-20260607.log       ← General app activity (14 day retention)
logs/security-20260607.log   ← Warnings+: failed logins, 403s, 429s (30 days)
logs/sql-20260607.log        ← EF Core SQL command logs (30 days)
```

Every request is logged with timing via `RequestLoggingMiddleware`. The `finally` block guarantees logging even on crashes — no blind spots in request observability.

### 9. Testing — Comprehensive Coverage

```powershell
dotnet test
```

Controllers, services, validators, pagination, guards — every layer is tested with **NSubstitute** for mocking and **FluentAssertions** for readable assertions:

- **Business rules**: duplicate applications, expired licenses, invalid state transitions
- **Controller responses**: status codes, response shapes, error handling
- **Edge cases**: null inputs, negative IDs, empty result sets

### 10. Database — EF Core Code-First with Migrations

All 15 entities configured via Fluent API, schema version-controlled as migrations:

```csharp
// Each relationship explicitly configured
builder.HasOne(a => a.ApplicationType)
       .WithMany()
       .HasForeignKey(a => a.ApplicationTypeId)
       .OnDelete(DeleteBehavior.Restrict);
```

Standard `DELETE` from a related table throws a foreign-key error — `Restrict` prevents accidental data loss at the database level.

### 11. Performance — Single-Round-Trip Pagination

```sql
SELECT ..., COUNT(*) OVER() AS TotalCount
FROM Applications
ORDER BY ApplicationId
OFFSET @p_0 ROWS FETCH NEXT @p_1 ROWS ONLY
```

One query returns both page data and total count via `COUNT(*) OVER()` — half the database round trips of naive pagination.

---

## Quick Start

```powershell
# Prerequisites: .NET 8 SDK, SQL Server

# Clone & build
git clone https://github.com/A7medMubarak/dvld-api.git
cd dvld-api
dotnet build

# Setup database (requires SQL Server)
dotnet ef database update --project src\DVLD.DataAccess --startup-project src\DVLD.Api
sqlcmd -S . -d DVLD-EFCore -i database\seed.sql

# Run
dotnet run --project src\DVLD.Api

# Swagger UI: https://localhost:7247/swagger

# Run tests
dotnet test
```

---

## Project Structure

```
DVLD/
├── src/
│   ├── DVLD.Contracts/      DTOs, enums, interfaces, validators, options
│   ├── DVLD.DataAccess/     EF Core DbContext, entities, repositories, migrations
│   ├── DVLD.Business/       Service implementations, business rules, guards
│   └── DVLD.Api/            Controllers, middleware, JWT auth, DI config
├── tests/
│   ├── DVLD.Business.Tests/ Service-level unit tests
│   └── DVLD.Api.Tests/      Controller integration tests
├── database/                Seed script
├── DVLD.sln
├── AGENTS.md                Architecture conventions & onboarding
└── README.md
```

---

## Let's Connect

This project demonstrates that I can design, build, test, and document a production-grade REST API — from database schema to authentication to deployment. I'm actively looking for a backend role where I can contribute to real systems, learn from experienced engineers, and write code that matters.

[A7medMubarak](https://github.com/A7medMubarak)
