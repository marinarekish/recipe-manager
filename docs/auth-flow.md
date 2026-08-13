# Recipe Manager — Passwordless Login (Login Code) Flow

> Design and current implementation notes. The service layer exists; the
> HTTP endpoints, email delivery, and JWT are planned follow-ups.

## Goal

Users log in with an email + one-time login code instead of a password.
The code is sent to the user's email (currently just logged in development).

## Flow

```
Client                         Server (AuthService)
  |   POST /auth/request-code (email)         |
  |------------------------------------------>| 1. look up user by email
  |                                           | 2. invalidate all active tokens
  |                                           | 3. generate 6-digit code
  |                                           | 4. store SHA-256 hash as LoginToken
  |                                           | 5. (dev) log the plaintext code
  |   POST /auth/verify-code (email, code)    |
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

## Current gaps / known issues

1. **No HTTP endpoints yet** — `IAuthService` is not exposed through a
   controller, so the flow cannot be called over the wire.
2. **No email sender** — the code is written to the log
   (`logger.LogInformation("Login code ... {Code}")`), development-only
   behavior flagged with a comment.
3. **User enumeration** — `RequestLoginCodeAsync` throws
   `KeyNotFoundException` when the email is unknown, so the response
   reveals whether an email is registered. Use a generic response for
   unknown and known emails alike.
4. **No rate limiting** — codes can be requested/attempted without bound.
   Add per-email/IP throttling and an attempt limit per token.
5. **Expired tokens accumulate** — add a cleanup job (e.g. delete
   `expires_at < now - retention`).
6. **Error signaling** — exceptions (`KeyNotFoundException`,
   `UnauthorizedAccessException`) are used for expected failures; a
   `Result<T>`-style outcome would map more cleanly onto HTTP 404/400/401.
7. **No session/JWT** — `AuthResponse` currently returns only the user
   profile (with roles); issuing a token is a planned follow-up.

## Planned next steps

- `AuthController` exposing `POST /auth/request-code` and
  `POST /auth/verify-code`.
- Real email provider behind an interface (the service currently logs).
- JWT issuance on successful verify + `[Authorize]` on protected endpoints
  (replacing the `CurrentUserId`/`IsAdmin` constants in `RecipeController`).
- Rate limiting and token-attempt limits.
