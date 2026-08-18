# Recipe Manager — Passwordless Login (Login Code) Flow

> Design and implementation notes. Both the service layer and HTTP
> endpoints are implemented.

## Goal

Users log in with an email + one-time login code instead of a password.
The code is sent to the user's email (currently just logged in development).

## Flow

```
Client                         Server (AuthService)
  |   POST /api/auth/request-code (email)       |
  |------------------------------------------>| 1. look up user by email
  |                                           | 2. invalidate all active tokens
  |                                           | 3. generate 6-digit code
  |                                           | 4. store SHA-256 hash as LoginToken
  |                                           | 5. (dev) log the plaintext code
  |   POST /api/auth/verify-code (email, code) |
  |------------------------------------------>| 6. find latest active token for user
  |                                           | 7. verify code hash (constant-time)
  |                                           | 8. mark token used
  |   <----- AuthResponse (User + roles) -----| 9. return user profile
```

## Data model — `LoginToken`

| Column            | Type              | Notes                                  |
| ----------------- | ----------------- | -------------------------------------- |
| `login_token_id`  | int (identity)    | PK                                     |
| `user_id`         | int               | FK → users, `ON DELETE CASCADE`        |
| `code_hash`       | varchar(255)      | SHA-256 hex of the plaintext code      |
| `created_at`      | timestamptz       | default `CURRENT_TIMESTAMP`            |
| `expires_at`      | timestamptz       | now + 10 minutes                       |
| `used_at`         | timestamptz (null)| null while active; set when consumed   |

A token is **valid** only while: `used_at IS NULL` and `expires_at > now`.

Indexes exist on `user_id`, `code_hash`, and `expires_at`.

## Implementation details (`AuthService`, `LoginCodeService`)

- **Code generation** — cryptographically random 6 digits:
  `RandomNumberGenerator.GetInt32(100_000, 1_000_000)`.
- **Hashing** — SHA-256 hex. The plaintext is never stored.
- **Verification** — constant-time comparison
  (`CryptographicOperations.FixedTimeEquals`) to avoid timing attacks.
- **One-time use** — the token is marked `used_at` on successful verify.
- **Single active session per user** — requesting a new code marks every
  still-active token as used, so only the newest code works.
- **Case/whitespace normalization** — email is `Trim().ToLowerInvariant()`
  on both request and verify.
- **Lifetime** — `LoginCodeLifetimeMinutes = 10`.

## Error signaling

Service methods return `Result` / `Result<T>` — no exceptions for expected
failures. Controllers use `ToActionResult()` to map to HTTP:

| Condition                          | Result               | HTTP |
| ---------------------------------- | -------------------- | ---- |
| Email not found                    | `NotFound`           | 404  |
| Code invalid or expired            | `Unauthorized`       | 401  |
| Success (request-code)             | `Ok`                 | 200  |
| Success (verify-code)              | `Ok(authResponse)`  | 200  |

See `result-convention.md` for the full mapping.

## Known issues / future work

1. **No email sender** — the code is written to the log
   (`logger.LogInformation("Login code ... {Code}")`), development-only
   behavior flagged with a comment.
2. **No rate limiting** — codes can be requested/attempted without bound.
   Add per-email/IP throttling and an attempt limit per token.
3. **Expired tokens accumulate** — add a cleanup job (e.g. delete
   `expires_at < now - retention`).
4. **No session/JWT** — `AuthResponse` currently returns only the user
   profile (with roles); issuing a token is a planned follow-up.
