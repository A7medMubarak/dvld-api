# Staff Password Reset (break-glass)

Use when a seeded password is lost. The API can't help (password change requires
login), so the reset goes directly against the database. Seed stores only BCrypt
hashes — they can't be reversed, only replaced.

> **The rule that matters:** the only valid check is
> `Verify(password, <hash SELECTed back from the database>)`.
> Verifying the hash you *generated* proves nothing — the UPDATE can store something
> else (wrong line pasted, one character corrupted in transit). And
> `LEN(PasswordHash) = 60` proves nothing: **every** bcrypt hash is exactly 60
> characters. Getting this wrong once cost a 5-hour production debugging session
> that blamed infrastructure while a bad hash sat in `Users`.

Run against the **production** DB. Prefer the one-shot script in step 2 — it
verifies itself end-to-end.

## 1. Pick new passwords (you, privately)

Requirements (enforced by the API afterwards too): 8+ characters with upper, lower,
digit, and special character (e.g. `Admin456!`). Do **not** put them in chat, files,
or git.

## 2. One-shot reset (recommended)

Edit the three placeholder passwords in `$accounts`, paste the whole block into
PowerShell, press Enter, then type the SQL password at the `SQL password:` prompt
(nothing echoes while typing — normal). It generates → updates → reads back →
verifies the **stored** values itself:

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

# Edit the three passwords before running (placeholders are skipped, never installed)
$accounts = [ordered]@{
  admin   = '<pick-per-account>'
  officer = '<pick-per-account>'
  viewer  = '<pick-per-account>'
}

$server = 'db70149.public.databaseasp.net'
$u = 'db70149'
$db = 'db70149'
$pwSql = Read-Host 'SQL password' -AsSecureString
$sqlPw = [Runtime.InteropServices.Marshal]::PtrToStringAuto(
  [Runtime.InteropServices.Marshal]::SecureStringToBSTR($pwSql))

$here = (& sqlcmd -S $server -U $u -P $sqlPw -d $db -Q "SET NOCOUNT ON; SELECT DB_NAME()")
if (-not ($here | Where-Object { "$_".Trim() -eq 'db70149' })) { "STOP: reached [$here] instead of db70149"; return }

foreach ($name in $accounts.Keys) {
  $password = $accounts[$name]
  if ($password -like '<*') { "$name : SKIPPED - edit the placeholder password first"; continue }
  $hash = [BCrypt.Net.BCrypt]::HashPassword($password, 11)
  $q = "SET NOCOUNT ON; UPDATE dbo.Users SET PasswordHash = '$hash' WHERE UserName = '$name'; SELECT PasswordHash FROM dbo.Users WHERE UserName = '$name';"
  try {
    $stored = (& sqlcmd -S $server -U $u -P $sqlPw -d $db -Q $q | Where-Object { $_ -match '^\$2a\$' } | Select-Object -First 1)
    $ok = [BCrypt.Net.BCrypt]::Verify($password, "$stored".Trim())
  } catch { $ok = "ERROR: $($_.Exception.Message)" }
  "$name : stored-verify = $ok"
}
"DONE - all three must be True before login"
```

Expected output:

```
admin : stored-verify = True
officer : stored-verify = True
viewer : stored-verify = True
DONE - all three must be True before login
```

## 3. The gate — before any login attempt

**All three `True` is mandatory before you attempt API login.** Any `False`,
`ERROR:`, or `SKIPPED` → stop, fix, re-run. Never "try the API anyway" — that is
how a wrong hash hides behind hours of infrastructure debugging.

## 4. Manual fallback (only if the one-shot script can't run)

### 4a. Hash it locally (your machine — plaintext never leaves it)

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

### 4b. Replace the hash — guarded (prod DB)

```sql
-- Safety guard: refuse to run anywhere except the production DB
DECLARE @here sysname = DB_NAME();

IF @here <> 'db70149'
BEGIN
    RAISERROR('WRONG DATABASE: you are on [%s] - switch to db70149 first.', 16, 1, @here);
    RETURN;
END

SET QUOTED_IDENTIFIER ON;

UPDATE dbo.Users SET PasswordHash = '<hash-from-4a>' WHERE UserName = 'admin';

-- Read back EXACTLY what is stored — 4c verifies this value
SELECT UserName, PasswordHash, IsActive
FROM dbo.Users WHERE UserName = 'admin';
```

### 4c. Verify the STORED value (same window as 4a)

```powershell
$stored = '<paste the PasswordHash from the 4b SELECT>'
[BCrypt.Net.BCrypt]::Verify('<the-password-you-hashed-in-4a>', $stored)   # must be True
```

`True` is required before any login attempt. `False` means the stored value and the
password disagree — re-run 4a→4b; do not proceed.

## 5. Run the smoke test now

`docs/smoke-test.md` Block 1, immediately. With the gate passed, step 2 (login)
must return 200; anything else is a fresh, small problem instead of a 5-hour one.

## 6. Finish securely

1. Store all three passwords in a password manager.
2. Officer/viewer can also be reset through the API
   (`PUT /api/users/{id}/password`) once admin is logged in — that path enforces
   step 1's password policy.
3. Never document plaintext in the repo.
