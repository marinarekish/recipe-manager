# Recipe Manager — Final Completion Checklist

> Internal / project-maintainer verification document. This is **not** user-facing documentation.
> Use it to manually confirm the current scope behaves as intended before considering the project complete.
> Items that are confirmed deferred or not yet implemented are marked explicitly rather than assumed.

---

## Backend functionality

### Endpoints
- [ ] `POST /api/auth/register`, `request-code`, `verify-code` respond correctly (see Authentication).
- [ ] Category, cuisine, and ingredient controllers expose GET, `get-or-create`, and (admin) DELETE.
- [ ] Recipe controller supports GET list / GET `me` / GET `{id}` / POST / PUT / DELETE.
- [ ] Favorite controller supports GET list / POST / DELETE.
- [ ] User controller supports `/me` (GET/PUT/DELETE) and admin-only GET list / GET `{id}` / PUT `{id}/role` / DELETE `{id}`.

### Validation
- [ ] Request DTOs enforce required fields, max lengths, ranges, and non-whitespace title where declared.
- [ ] Email is normalized (`Trim().ToLowerInvariant()`) on create/update and auth.
- [ ] Duplicate email returns a friendly validation/conflict result.
- [ ] Duplicate ingredient within a recipe request is rejected.

### Error / status codes
- [ ] All controllers route responses through `ResultExtensions.ToActionResult()` and return consistent codes (200/201/204/400/401/403/404/409).
- [ ] Confirm the 201 Location headers point at the created resource (recipe vs favorites endpoints currently differ — verify intent).

### CRUD behavior
- [ ] A user can create, read, update, and delete their own recipes.
- [ ] Ingredient/cuisine/category references can be supplied by id **or** name; name resolution creates missing lookup values.
- [ ] Updating a recipe preserves fields not included in a partial update.

### Ownership rules
- [ ] A user cannot update or delete a recipe they do not own (returns 403).
- [ ] An administrator can update/delete any recipe.

### Admin rules
- [ ] Only `Administrator` can list/view all users, assign roles, delete users, and delete reference data.
- [ ] Last-administrator deletion returns a validation error.
- [ ] Removing an administrator role from the last administrator is blocked (validation error).

---

## Authentication

- [ ] Registration creates a user with the default `User` role and does **not** return a JWT.
- [ ] Duplicate registration email returns 409 and a clear message.
- [ ] `request-code` returns 404 for an unknown email and a masked success message otherwise.
- [ ] A 6-digit code is generated and stored only as a SHA-256 hash.
- [ ] Requesting a new code invalidates the user's previously active codes.
- [ ] `verify-code` succeeds with the current code and returns a JWT + user profile.
- [ ] Invalid code returns 401.
- [ ] Expired code (after 10 minutes) returns 401.
- [ ] A code cannot be reused after successful verification.
- [ ] JWT validates issuer, audience, lifetime, and signing key; requires a key ≥ 32 characters at startup.

### Session behavior
- [ ] Frontend stores the token + user in `localStorage` and restores the session on reload.
- [ ] Logout clears stored session and returns to the login page.
- [ ] Expired-token behavior confirmed (currently local error message; global 401 handling is deferred — see Frontend).

---

## Authorization / RBAC

Matrix of effective access (anonymous / `User` / `Administrator`):

| Capability | Anonymous | User | Administrator |
|---|---|---|---|
| Register / request code / verify code | yes | yes | yes |
| View recipes | no | yes | yes |
| Create recipe | no | yes | yes |
| Edit own recipe | no | yes | yes |
| Edit another's recipe | no | no | yes |
| Delete own recipe | no | yes | yes |
| Delete another's recipe | no | no | yes |
| Manage own favorites | no | yes | yes |
| Manage own account | no | yes | yes |
| List/view/delete users, assign roles | no | no | yes |
| Delete reference data | no | no | yes |
| **Create** reference data (`get-or-create`) | yes | yes | yes *(intentional — user-extensible lookup data)* |

- [ ] Anonymous access is limited to the auth endpoints and reference-data GET/`get-or-create`.
- [ ] Ownership is enforced in the service layer (not only controllers).
- [ ] Second user cannot modify the first user's recipes, favorites, or account.

---

## User deletion and data integrity

When deleting a user, manually verify:

- [ ] Their authored recipes are deleted.
- [ ] Recipes authored **by other users** are untouched.
- [ ] Their favorites on recipes owned by others are removed.
- [ ] Favorites that other users placed on the deleted user's recipes are removed (recipe → favorite cascade).
- [ ] Their `users_roles` and `login_tokens` rows are removed (cascade).
- [ ] No unexpected FK/restrict errors surface.
- [ ] Deleting the last administrator is rejected.

Record the mechanism for each (database cascade/restrict vs explicit `UserService.DeleteUserAsync` logic) so the README stays accurate.

---

## Database

- [ ] Migrations apply cleanly from a fresh database (`dotnet ef database update`).
- [ ] Migrations seed roles (`Administrator`, `User`), categories, cuisines, and ingredients.
- [ ] Confirm how the initial `Administrator` user is created in practice, and document it. *(Repository note: only **roles** and reference data are seeded; users are not seeded. Creating an admin still requires an account + role grant — confirm the current intended mechanism and update docs if changed.)*
- [ ] Unique indexes exist on `users.email`, `roles.name`, and reference-data names.
- [ ] Check constraints exist for servings/prep/cook time `> 0` and ingredient `amount > 0`.
- [ ] Required vs optional columns and max lengths match the entities.
- [ ] Delete behaviors match the documented table (recipe author Restrict; join tables Cascade).

---

## Frontend

- [ ] `ng build` completes without errors (strict TS config).
- [ ] Routes resolve: `/login`, `/register`, `/verify?email=…`, and the protected area.
- [ ] Anonymous users hitting a protected route are redirected to `/login`.
- [ ] Authenticated users can enter the application normally.
- [ ] Login → request-code → `/verify?email=…` → verify → application flow works.
- [ ] `authGuard` protects the application area; `adminGuard` protects the admin route.
- [ ] `authInterceptor` attaches the Bearer token to API calls.
- [ ] Session restores after a page reload.
- [ ] Logout clears storage and returns to login.
- [ ] Register, login, and verify screens show validation and error states (incl. duplicate email, 401 invalid/expired code, 404 no account).
- [ ] Recipe explore, my-recipes, detail, create/edit form, favorites, and profile screens render and operate against the API.
- [ ] Admin user screen is a recognized placeholder (deferred), not a finished feature.
- [ ] Recipe card/grid show loading, error, and empty states; image falls back to a placeholder when `imageUrl` is absent.
- [ ] Responsive layout and Material/SCSS consistency at common viewport widths.

### Deferred (do not mark passing)
- [ ] TODO: `returnUrl` is written by `authGuard` but not consumed; after login the user is sent to `/recipes`. Decide desired deep-link behavior.
- [ ] TODO: global expired-JWT (401) handling is not implemented; components show local errors.
- [ ] TODO: frontend tests not implemented.

---

## Testing

### Automated
- [ ] Backend service tests pass: `dotnet test` from `backend/`.
- [ ] Important business rules covered: auth (request/verify, invalid/expired code), recipe ownership, last-admin protection, favorite add/duplicate/remove.
- [ ] TODO: add controller/integration tests (endpoint → status-code mapping).
- [ ] TODO: add tests for detailed user-deletion cascade behavior (authored recipes, other users' favorites).
- [ ] TODO: add tests for reference-data `get-or-create`.
- [ ] TODO: frontend tests not implemented.

### Manual smoke checks (see Final manual smoke test)
- [ ] Register → login-code → verify → enter recipe area.
- [ ] Negative cases: invalid code, expired code, duplicate email, unauthorized edit.

---

## Security

- [ ] No secrets, tokens, or credentials are tracked in Git.
- [ ] `appsettings.Development.json` is git-ignored; `.example` file contains placeholders only.
- [ ] JWT validation (issuer, audience, lifetime, signing key) is enabled; key-length guard runs at startup.
- [ ] Authorization attributes/guards are present on protected endpoints/routes.
- [ ] Ownership is enforced (no horizontal privilege escalation).
- [ ] CORS allows only intended local development origins (`http://localhost:4200` for Angular). *(Repository note: `:5173`/Vite origin is obsolete since the React frontend was removed; it remains in the example config and docs — confirm whether to remove it as a follow-up.)*
- [ ] No passwords are stored anywhere (passwordless design).
- [ ] Login codes are hashed, constant-time compared, single-use, time-limited, and invalidated on newer requests.
- [ ] Plaintext login code is only logged for development (confirm it is not exposed in a production-facing log path).
- [ ] Error/log output does not leak sensitive data beyond intended development hints.
- [ ] AOT/template escaping is preserved (no `[innerHTML]`/`bypassSecurityTrust` usage found).

Do not add security requirements beyond what the application actually implements.

---

## Documentation

- [ ] Root `README.md` reflects current code and confirmed decisions.
- [ ] `docs/api.md` matches the implemented endpoints.
- [ ] `docs/auth-flow.md` matches the implemented auth behavior.
- [ ] `docs/frontend-runbook.md` is up to date with the shipped frontend *(repository note: it still lists guards/interceptors/recipe screens as "not implemented" — update).*
- [ ] `docs/backend-runbook.md` matches current projects/migrations.
- [ ] Known limitations and roadmap sections are present and accurate.
- [ ] Documented verification/run commands still work.

---

## Repository hygiene

- [ ] No obsolete React/Vite source files remain.
- [ ] Any stale Vite (`:5173`) CORS references are resolved or explicitly tracked as a follow-up.
- [ ] No debug code, temporary test calls, or leftover scaffolding.
- [ ] No generated build artifacts (e.g. `dist/`, `bin/`, `obj/`) expected in source control.
- [ ] No secrets committed.
- [ ] Branch/PR state reviewed manually; changes ready for a single final commit.
- [ ] Documentation changes and code changes are staged intentionally.

---

## Final manual smoke test

Run through once against a fresh local setup:

1. Start PostgreSQL on `localhost:5432`.
2. Apply migrations (`dotnet ef database update`).
3. Start the backend (`cd backend && dotnet run --project RecipeManager.Api`).
4. Start the frontend (`cd frontend && ng serve`).
5. Open `http://localhost:4200` unauthenticated → confirm the login/request-code flow is shown (not the protected area).
6. Register a new user.
7. Request a login code (read it from the API logs in development).
8. Verify the code.
9. Enter the recipe area and confirm the JWT session.
10. Create a recipe; edit it; delete it.
11. Add and remove a favorite.
12. Confirm a second (non-admin) user cannot edit/delete the first user's recipe (403).
13. Verify admin restrictions (user list/role assignment/delete are admin-only).
14. Verify user-deletion semantics per the checklist above.
15. Log out.
16. Reload the page and confirm the expected authentication behavior (session restores while the token is valid; anonymous users are redirected to login).

Mark any step that is not yet implemented as TODO rather than assuming it passes.
