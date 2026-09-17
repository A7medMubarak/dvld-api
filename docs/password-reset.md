# Staff Password Reset (break-glass)

Use when a seeded password is lost. The API can't help (password change requires
login), so the reset goes directly against the database. Seed stores only BCrypt
hashes — they can't be reversed, only replaced.

Run **after** seeding, against the **production** DB. One account at a time.

## 1. Pick a new password (you, privately)

Requirements (enforced by the API afterwards too): 8+ characters with upper, lower,
digit, and special character. Do **not** put it in chat, files, or git.

## 2. Hash it locally (your machine — plaintext never leaves it)

PowerShell, any folder. Typing is masked; only the hash is printed:

```powershell
$pkgs = "$env:USERPROFILE\.nuget\packages"
$map = @{
  "System.Memory" = "$pkgs\system.memory\4.5.5\lib\netstandard2.0\System.Memory.dll"
  "System.Runtime.CompilerServices.Unsafe" = "$pkgs\system.runtime.compilerservices.unsafe\6.0.0\lib\netstandard2.0\System.Runtime.CompilerServices.Unsafe.dll"
  "System.Buffers" = "$pkgs\system.buffers\4.5.1\lib\netstandard2.0\System.Buffers.dll"
  "System.Numerics.Vectors" = "$pkgs\system.numerics.vectors\4.5.0\lib\netstandard2.0\System.Numerics.Vectors.dll"
}
[System.AppDomain]::CurrentDomain.add_AssemblyResolve({
  param($s,$e) $n = ($e.Name -split ",")[0]
  if ($map.ContainsKey($n)) { return [System.Reflection.Assembly]::LoadFrom($map[$n]) }
  return $null
})
Add-Type -LiteralPath "$pkgs\bcrypt.net-next\4.0.3\lib\netstandard2.0\BCrypt.Net-Next.dll"
$pw = Read-Host "New password" -AsSecureString
$plain = [Runtime.InteropServices.Marshal]::PtrToStringAuto(
  [Runtime.InteropServices.Marshal]::SecureStringToBSTR($pw))
[BCrypt.Net.BCrypt]::HashPassword($plain, 11)
$plain = $null
```

The output starts with `$2a$11$` (same format as `seed.sql`, work factor 11).
Requires the `bcrypt.net-next` 4.0.3 + support packages in the local NuGet cache
(present after building the solution).

## 3. Replace the hash (one statement, prod DB)

```sql
UPDATE Users SET PasswordHash = '<hash-from-step-2>' WHERE UserName = 'admin';
```

## 4. Finish securely

1. Log in as `admin` with the new password (smoke test step 2).
2. Reset `officer` / `viewer` normally via `PUT /api/users/{id}/password`.
3. Store all three in a password manager. Never document plaintext in the repo.
