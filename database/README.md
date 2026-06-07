# Database

## Usage

1. Apply all migrations:
   ```powershell
   dotnet ef database update --project src\DVLD.DataAccess --startup-project src\DVLD.Api
   ```

2. Seed the database:
   ```powershell
   sqlcmd -S . -d DVLD-EFCore -i database\seed.sql
   ```

> **⚠️ Seed must run AFTER all migrations are applied.** Running it on a partial schema will fail with cryptic errors.
