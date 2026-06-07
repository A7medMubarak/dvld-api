# AGENTS.md — DVLD

## Project structure

5 .NET 8 projects, root `DVLD.sln`:

```
DVLD/
├── database/              SQL seed script
├── src/
│   ├── DVLD.Contracts/    Shared types (Dtos, Requests, Validators, Enums, Exceptions, Options)
│   ├── DVLD.DataAccess/   EF Core DbContext, Entities, Repositories, Migrations, Entity configs
│   ├── DVLD.Business/     Service interfaces + implementations
│   └── DVLD.Api/          ASP.NET Core Web API (entrypoint)
├── tests/
│   ├── DVLD.Business.Tests/
│   └── DVLD.Api.Tests/
├── DVLD.sln               Single solution at root
└── AGENTS.md
```

Project reference graph (from `.csproj` files):
- `DVLD.Api` → `DVLD.Contracts` + `DVLD.Business` + `DVLD.DataAccess`
- `DVLD.Business` → `DVLD.Contracts`
- `DVLD.DataAccess` → `DVLD.Contracts`
- Repository interfaces live in `src/DVLD.Contracts/Interfaces/Repositories/`

## Build & run

```powershell
# Build everything
dotnet build .\DVLD.sln

# Run the API
dotnet run --project .\src\DVLD.Api\DVLD.Api.csproj

# Run all tests
dotnet test .\DVLD.sln

# Run business tests only
dotnet test .\tests\DVLD.Business.Tests\DVLD.Business.Tests.csproj

# Run API controller tests only
dotnet test .\tests\DVLD.Api.Tests\DVLD.Api.Tests.csproj

# Add migration (from root)
dotnet ef migrations add <Name> --project .\src\DVLD.DataAccess --startup-project .\src\DVLD.Api

# Apply pending migrations
dotnet ef database update --project .\src\DVLD.DataAccess --startup-project .\src\DVLD.Api

# Seed the database (after migrations)
sqlcmd -S . -d DVLD-EFCore -i database\seed.sql
```

No lint, format, or CI tooling is configured.

## Database

- SQL Server, database: `DVLD-EFCore`, connection in `src/DVLD.Api/appsettings.json`
- 3 migrations exist: `20260407105918_Initial`, `20260516225744_AddCreatedByUserIDToPerson`, `20260518011046_AddRefreshTokensTable`

## Architecture conventions

- **Async all the way**: every service & repository method accepts `CancellationToken cancellationToken = default`
- **Repository pattern**: repositories map between EF Core entities and Contracts DTOs using `Expression<Func>` projection (`EntityToDto`)
- **Services** orchestrate business logic, validate inputs, throw `ArgumentException` / `KeyNotFoundException` / `InvalidOperationException`
- **Global exception handler** (`src/DVLD.Api/Middleware/GlobalExceptionHandler.cs`) maps exceptions to status codes and logs. Maps: `ArgumentException`/`ValidationException` → 400, `KeyNotFoundException` → 404, `InvalidOperationException` → 409, `UnauthorizedAccessException` → 401, default → 500
- **FluentValidation auto-validation** via `SharpGrip.FluentValidation.AutoValidation.Mvc` — validators live in `src/DVLD.Contracts/Validators/`, registered from `AddValidatorsFromAssemblyContaining<LicenseClassWriteRequest>()`
- **DI registration** centralized in `src/DVLD.Api/Extensions/ServiceCollectionExtensions.cs`
- **JWT auth** configured with `JwtSettings` from `appsettings.json` (placeholder secret — replace before production)
- **CORS** policy named `"DVLDApiCorsPolicy"`, origins: `https://localhost:7247`, `http://localhost:5003`
- **ICurrentUserService** interface in `src/DVLD.Business/Interfaces/Services`, implementation in `src/DVLD.Api/Services/` — reads `UserId` and `Role` from JWT claims
- **Custom Guard utility** in `src/DVLD.Business/Common/Validation/Guard.cs` (null/empty/range checks)
- **Custom `DVLD.Contracts.Exceptions.ValidationException`** — carries `Errors` dictionary, handled specially by the global exception handler (returns 400 with `ValidationProblemDetails`)

## Gotchas

| # | Issue | Location |
|---|---|---|
| 1 | `appsettings.json` is an **EmbeddedResource** (not Content). Modifying at runtime won't work — must rebuild. | `src/DVLD.Api/DVLD.Api.csproj:9-16` |
| 2 | ~~`ICountryService`/`CountryService` exist but are **missing from DI registration** in `AddServices()`. Injection at runtime will throw.~~ **FIXED**: Added `services.AddScoped<ICountryService, CountryService>()`. | `src/DVLD.Api/Extensions/ServiceCollectionExtensions.cs` |
| 3 | ~~`PasswordService` is a `static` class with no interface — inconsistent with the rest of the DI pattern.~~ **FIXED**: Extracted `IPasswordService` interface, made `PasswordService` non-static, registered in DI. | `src/DVLD.Business/Services/PasswordService.cs` |
| 4 | ~~`AppDbContext.cs` has `FindAsync(int)` that throws `NotImplementedException`~~ **FIXED**: Removed dead code (was never called). | `src/DVLD.DataAccess/Data/AppDbContext.cs` |
| 5 | ~~`PersonDto.cs` has `implicit operator PersonDto(Task<PersonDto?>)` that throws `NotImplementedException`~~ **FIXED** (no longer present — operator was removed). | `src/DVLD.Contracts/Dtos/Person/PersonDto.cs` |

## Secrets & Security

- JWT `SecretKey` in `appsettings.json` is a **placeholder** — must be replaced before any non-local deployment
- For **development**, use `dotnet user-secrets`:
  ```powershell
  dotnet user-secrets init --project .\src\DVLD.Api\DVLD.Api.csproj
  dotnet user-secrets set "JwtSettings:SecretKey" "<your-256-bit-key>" --project .\src\DVLD.Api\DVLD.Api.csproj
  ```
- For **production**, use environment variables or Azure Key Vault
- Refresh tokens are SHA-256 hashed before storage — raw tokens are never persisted

## Controller conventions

- Route pattern: `api/<entity>` (lowercase, plural, kebab-case), e.g. `api/people`, `api/test-types`, `api/local-driving-license-applications`
- File naming: `XxxController.cs` (no `API` infix)
- Class naming: matches filename, e.g. `PeopleController`, `AuthController`
- Namespace: `DVLD.Api.Controllers` (consistent across all 14 controllers)

## Key packages

| Package | Version | Used in |
|---|---|---|
| EF Core SqlServer | 8.0.14 | DVLD.Api, DVLD.DataAccess |
| BCrypt.Net-Next | 4.0.3 | DVLD.Business (password hashing) |
| FluentValidation | 12.1.1 | DVLD.Contracts |
| SharpGrip.FluentValidation.AutoValidation.Mvc | 2.0.0 | DVLD.Api |
| JwtBearer auth | 8.0.26 | DVLD.Api |
| Swashbuckle | 6.6.2 | DVLD.Api |
| Serilog.AspNetCore | 8.0.3 | DVLD.Api |
| Serilog.Sinks.File | 6.0.0 | DVLD.Api |
| Serilog.Sinks.Async | 2.1.0 | DVLD.Api |

## Naming conventions

| Rule | Standard | Example |
|---|---|---|
| ID identifiers | `Id` suffix (lowercase `d`) | `PersonId`, `ApplicationId`, `CreatedByUserId` |
| DTO classes | `Dto` suffix | `PersonDto`, `ApplicationDto`, `UserDto` |
| Namespace | `DVLD.Contracts.Dtos.*` | `DVLD.Contracts.Dtos.Person` |
| Enum prefix | `en` prefix | `enGender`, `enApplicationStatus`, `enTestType` |
| Async methods | `Async` suffix on `Task`-returning methods only | `GetByIdAsync` — not on `void` methods |
| Method parameters | camelCase | `personId`, `applicationId`, `ct` |
| Property setters | `{ get; set; }` order | Not `{ set; get; }` |

## Entity config & naming

- EF Core Fluent API configs in `src/DVLD.DataAccess/Data/Confiq/` (one file per entity). Applied via `modelBuilder.ApplyConfigurationsFromAssembly()`.
- `src/DVLD.Contracts/Common/Enums/` — all use `en` prefix: `enGender`, `enApplicationStatus`, `enTestType`, etc.

## Rate limiting

3 policies configured via `AddRateLimiting()` in `ServiceCollectionExtensions.cs`:

| Policy | Key | Limit | Applied to |
|---|---|---|---|
| **Global** | Authenticated: `UserId` | 100 req/min | All `[Authorize]` endpoints |
| **Global** | Anonymous: IP | 30 req/min | Every endpoint (fallback) |
| **Auth** | IP | 5 req/min | `POST /api/auth/login`, `POST /api/auth/refresh` |
| **Sensitive** | Authenticated: `UserId` | 10 req/min | `PUT /api/users/{id}/password` |
| **Sensitive** | Anonymous: IP | 3 req/min | Same endpoint (unauthenticated fallback) |

- Middleware order: `UseAuthentication` → `UseRateLimiter` → `UseAuthorization`
- 429 response returns generic `ProblemDetails` with no internal details (no retry-after, no policy name, no partition key)
- `[EnableRateLimiting("PolicyName")]` works on both controller and action level; action overrides controller
- All `[Authorize]` endpoints without an explicit attribute inherit the Global policy
- Anonymous requests to `[Authorize]` endpoints consume the Global anonymous limit before being rejected by auth
