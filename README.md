# Recipe Manager

A full-stack web application for storing, organizing, and browsing cooking recipes. Users authenticate without passwords, create and manage their own recipes, save favorites, and browse a shared collection. Administrators maintain users, content, and reference data.

The application is an Angular 19 SPA backed by an ASP.NET Core (.NET 10) REST API and PostgreSQL. Beyond product functionality, it is a deliberate learning and engineering project covering layered backend design, relational modeling, authentication and authorization, and frontend/backend integration.

---

## 2. Technical goals

This project is intentionally built as an engineering exercise. The codebase is structured to demonstrate and practice:

- **ASP.NET Core / .NET 10** API design and dependency injection;
- **Entity Framework Core** with PostgreSQL, including migrations, model configuration, and database-level integrity rules;
- **Authentication and authorization** — passwordless login codes, JWT, roles, and ownership enforcement;
- **Angular 19** standalone component architecture, routing, guards, and interceptors;
- **Layered application design** and separation of concerns;
- **Testing** at the backend service layer;
- **API/frontend integration** including a consistent JSON contract and CORS.

The goal is a working, readable application with honest documentation of both intentional choices and known gaps — not a generic tutorial project.

---

## 3. Features

### Implemented

- **Passwordless registration and login** — accounts are created with name + email; authentication uses a one-time 6-digit login code, never a password.
- **Recipes** — create, read, update, and delete; organized by category and cuisine; each recipe has preparation/cooking time, servings, instructions, and a list of ingredients with amount and unit.
- **Ownership rules** — a regular user can edit/delete only recipes they authored; administrators can edit/delete any recipe.
- **Favorites** — users can save and remove recipes to/from their personal favorites list.
- **Categories, cuisines, and ingredients** — reference data. Lookup values are seeded and can be created on the fly when a recipe references a new name (see Engineering decisions).
- **User roles** — `User` (default) and `Administrator`.
- **Administrator capabilities (backend)** — list/view/delete users, assign or replace a user's role, delete any recipe, and delete reference-data records.
- **Recipe images via URL** — each recipe stores an optional `imageUrl`; the frontend renders it with a fallback placeholder when absent.
- **Validation and error handling** — data-annotation validation on requests, database check constraints, and a `Result<T>` pattern that maps to consistent HTTP status codes.

### Planned / not yet implemented

- **Admin user-management UI** — the backend and route exist, but the frontend screen is currently a placeholder.
- **Recipe search / filtering / sorting / pagination**.
- **Frontend test suites** (see Testing and Known limitations).

---

## 4. Tech stack

### Backend

| Technology            | Detail                                                                   |
| --------------------- | ------------------------------------------------------------------------ |
| ASP.NET Core          | .NET 10.0, Web SDK                                                       |
| Entity Framework Core | 10.0.10 (Npgsql provider 10.0.3)                                         |
| Database              | PostgreSQL (recent/current release)                                      |
| Authentication        | JWT Bearer (`Microsoft.AspNetCore.Authentication.JwtBearer` 10.0.11)     |
| Mapping               | AutoMapper 16.2.0                                                        |
| API documentation     | Swashbuckle / Swagger 10.2.3 (Development only)                          |
| Testing               | xUnit 2.9.3, NSubstitute 5.3.0, EF Core InMemory 10.0.10, coverlet 6.0.4 |

### Frontend

| Technology | Detail                                                       |
| ---------- | ------------------------------------------------------------ |
| Angular    | 19.2 (standalone APIs)                                       |
| TypeScript | 5.7 (strict mode)                                            |
| UI         | Angular Material + CDK 19.2.19 (prebuilt `azure-blue` theme) |
| RxJS       | 7.8                                                          |
| Styling    | SCSS                                                         |
| Testing    | Jasmine + Karma configured, **no test files yet** (deferred) |

---

## 5. Architecture

### Backend — layered, intentionally pragmatic

Four projects with a single dependency direction:

```
RecipeManager.Api            ASP.NET Core host: controllers, DI, auth pipeline, JWT, CORS, Swagger
      │
RecipeManager.Application    Services, interfaces, contracts/DTOs, AutoMapper profiles, Result<T>, settings
      │
      ├── RecipeManager.Domain           Entities (no external dependencies)
      └── RecipeManager.Infrastructure   DbContext, EF configurations, migrations
```

- **Controllers** are thin: they extract the current user id from JWT claims, call a service, and map the returned `Result<T>` to an HTTP response via `ResultExtensions.ToActionResult()`.
- **Application services** hold business logic — ownership checks, role assignment, last-administrator protection, recipe/cuisine/category/ingredient resolution, and validation that depends on application state (e.g. duplicate detection). This is why logic lives in the Application layer rather than inside controllers: it is testable without a web host and reusable.
- **Domain** contains persistence-oriented entities plus navigation collections. Entity mapping to the database is kept fully separate via `IEntityTypeConfiguration<T>` classes in Infrastructure, keeping entities free of persistence concerns.
- **Persistence** uses EF Core with a single `ApplicationDbContext`. There is intentionally no repository layer: services work against the `DbContext` directly. `Application` references `Infrastructure` for this reason.

This is **"Clean Architecture-inspired," not strict Clean Architecture.** The layer separation, dependency direction, and separation of persistence concerns are deliberate, but the project is a solo application and consciously trades some textbook purity (e.g. no repository abstraction) for directness. This pragmatism keeps the code small while still demonstrating the layering concepts.

A full local setup guide lives in `docs/backend-runbook.md`.

### Frontend — Angular 19 standalone

- **Standalone components** (no NgModules), bootstrapped via `bootstrapApplication` in `src/main.ts`.
- **Feature/core/shared structure:**
  - `core/` — cross-cutting auth (service, guards, interceptor, models) and route-level helpers.
  - `shared/components/` — reusable presentational components (`recipe-card`, `recipe-grid`, `placeholder`).
  - `features/` — screen-level features (`auth`, `layout`, `recipes`, `favorites`, `profile`), each split into `data/` (API services + models) and `pages/` (view components).
- **Router** — routes are defined in `app.routes.ts`; guarded by `authGuard` (protected area), `adminGuard` (admin-only area), and a `recipeIdGuard` (id parameter validation).
- **HTTP** — configured via `provideHttpClient(withInterceptors([authInterceptor]))`; the interceptor attaches the stored JWT as `Authorization: Bearer …`.
- **Auth state** — an `AuthService` holds the current user in an Angular signal and persists the JWT + user profile in `localStorage` for session restoration.
- **UI** — Angular Material imported per-component and SCSS component styles.

---

## 6. Authentication and authorization

### Authentication flow (passwordless)

```
register → request login code → verify code → JWT → authenticated application
```

1. **Register** — `POST /api/auth/register` (public). Creates a user with the default `User` role. No password is ever involved.
2. **Request login code** — `POST /api/auth/request-code` (public). The server generates a cryptographically random 6-digit code and stores only its SHA-256 hash. The code expires after 10 minutes.
3. **Verify code** — `POST /api/auth/verify-code` (public). The code is compared in constant time, marked as used, and a JWT is issued.
4. **JWT session** — the client stores the token and user profile in `localStorage` and sends the token on subsequent requests.

Implementation notes:

- A token is valid only while it is unused and not expired; requesting a new code invalidates all previously issued active codes for that user.
- **Development only:** the plaintext login code is written to the API console logs so the flow is testable without an email provider. There is no email delivery.
- The JWT carries a minimal claim set: `sub` (user id), `email`, `jti`, and the user's `role` claim(s). Validation (issuer, audience, lifetime, signing key) is configured in `Program.cs`, with a startup guard that requires a signing key of at least 32 characters.
- Session/token is stored in `localStorage` on the frontend — a deliberate simplicity trade-off for a client-side SPA (no HttpOnly cookie / refresh-token flow; see Known limitations).

### Authorization / RBAC

- **`User`** (default) — manages their own recipes, favorites, and profile.
- **`Administrator`** — additionally manages other users (list/view, delete, assign/replace roles), can edit/delete any recipe, and can delete reference-data records.

Authorization is enforced at the API through `[Authorize]` and `[Authorize(Roles = "Administrator")]`, with ownership checks performed in the service layer (a non-admin may only edit/delete their own recipes). The Administrator role represents a distinct **maintenance responsibility**, not merely a more powerful regular user: regular users consume and maintain their own content, while administrators govern content and users platform-wide.

On the frontend, `authGuard` protects the application area and redirects anonymous users to `/login`; `adminGuard` restricts the admin route to users holding the `Administrator` role.

---

## 7. Database and data integrity

The schema is PostgreSQL with 10 tables. Key entities: `users`, `roles`, `users_roles`, `recipes`, `categories`, `cuisines`, `ingredients`, `recipe_ingredients`, `user_favorites`, and `login_tokens`. Reference data (categories, cuisines, ingredients) and roles are seeded by EF migrations; users are not seeded.

Important relationships and their delete behavior:

| Relationship                                            | Delete behavior | Rationale                                                      |
| ------------------------------------------------------- | --------------- | -------------------------------------------------------------- |
| `recipe.author` → `users`                               | Restrict        | Do not silently remove authored content via a plain FK cascade |
| `recipe.category` / `recipe.cuisine` → reference tables | Restrict        | Prevent removing reference data still in use                   |
| `recipe_ingredients.ingredient` → `ingredients`         | Restrict        | Same                                                           |
| `recipe_ingredients.recipe` → `recipes`                 | Cascade         | Ingredient lines have no meaning without the recipe            |
| `user_favorites` → `users` and → `recipes`              | Cascade         | Join rows are meaningless without either side                  |
| `login_tokens.user` → `users`                           | Cascade         | Auth tokens die with the account                               |
| `users_roles.user` → `users`                            | Cascade         | Role assignment dies with the account                          |

Composite primary keys model the `recipe_ingredients` and `user_favorites` join tables; `users_roles` uses a composite key as well.

### Database-level integrity

- Unique indexes on `users.email`, `roles.name`, and each reference-data name.
- Check constraints: `servings > 0`, `prep_time_minutes > 0`, `cook_time_minutes > 0`, and ingredient `amount > 0`.
- Required vs optional columns and max lengths are enforced in the EF configurations.

### User deletion semantics

Deleting a user is handled by a deliberate combination of **explicit application logic** and **database cascade/restrict behavior** (`UserService.DeleteUserAsync`). Because `recipe → author` is Restrict, the service explicitly:

1. deletes the recipes authored by that user;
2. clears the deleted user's favorites on recipes **owned by other users**;
3. leaves recipes authored by other users untouched.

Separately, the database cascade rules remove:

- favorites that **other users** created on the deleted user's recipes (driven by the recipe removal and `user_favorites → recipes` cascade);
- the deleted user's remaining join rows (`users_roles`, `login_tokens`, and their other `user_favorites` via `user_favorites → users` cascade).

The service also prevents deleting (or stripping the role from) the **last administrator**.

A full entity/relationship description is in `docs/domain_model.md`.

---

## 8. Interesting engineering decisions

- **Passwordless authentication instead of passwords.** No password storage or hashing at all. A random 6-digit code is stored only as a SHA-256 hash, compared in constant time, single-use, and time-limited. This removes the most sensitive part of a typical auth system from the codebase while still demonstrating real authentication engineering.
- **`Restrict` on the recipe author combined with explicit deletion logic.** Rather than relying on a `Cascade` that would silently delete authored recipes, the author FK uses `Restrict`, and `UserService.DeleteUserAsync` explicitly orchestrates what deletion should mean (see User deletion semantics). This makes the intended behavior explicit and auditable.
- **Cascade behavior for join tables.** `recipe_ingredients`, `user_favorites`, `users_roles`, and `login_tokens` use cascade deletes because their rows are meaningless without their parents — a clean division from the explicit recipe-author handling.
- **Public, idempotent `get-or-create` for reference data.** Any authenticated user creating or editing a recipe may extend the set of categories, cuisines, and ingredients. This is a deliberate domain decision: lookup data is **user-extensible**, and `get-or-create` prevents duplicate values from being introduced by free-text names. It is not an RBAC oversight; making these endpoints admin-only was considered and rejected. Administrators still have separate management responsibilities (notably deletion) where applicable.
- **RBAC with a maintenance role.** The two-role model cleanly separates "regular users manage their own content" from "administrators govern content and users," including last-administrator protection.
- **Pragmatic layered architecture.** Four projects with clear dependency direction, persistence separated from entities, but no repository layer because Application works against the `DbContext` directly — an intentional trade of some textbook purity for a smaller, more direct codebase.
- **`Result<T>` error handling.** Services return explicit result states (OK, NotFound, Validation, Conflict, Forbidden, Unauthorized, NoContent) that map cleanly to HTTP status codes, instead of relying on thrown exceptions for expected failures. This centralizes and makes the API's status-code conventions consistent.
- **localStorage session persistence.** The JWT and user profile are kept in `localStorage` for session restoration across reloads. This is simpler than cookie-based storage or refresh tokens and is a documented trade-off for a client-side SPA.
- **Recipe images as URLs with a fallback.** Rather than an upload subsystem, each recipe stores an optional `imageUrl`; the UI shows a styled placeholder when it is absent. This keeps scope small and was an explicit simplification.
- **Explicit ownership checks in the service layer** rather than only in controllers, so authorization rules are testable without an HTTP host.

---

## 9. Testing

### Backend

The `RecipeManager.Tests` project contains **service-layer unit tests** using xUnit, NSubstitute, and EF Core InMemory. Current coverage focuses on:

- `AuthService`: request-code success/not-found; verify-code success, invalid code, expired code.
- `UserService`: user lookup, profile update, duplicate email (create + update), default role on registration, last-administrator protection (role removal and deletion), regular-user deletion.
- `RecipeService`: create, get by id, ownership on update (and forbidden for others), delete own recipe, admin deleting another user's recipe.
- `FavoriteService`: add, duplicate add (conflict), remove, list.

Backend tests exercise the service layer directly against an in-memory `DbContext` via shared fixtures (`TestDataSeeder`, `TestDbContextFactory`).

### Frontend

Jasmine + Karma are configured (`ng test`), but **no spec files are implemented yet**. This is deferred work, not a claim of coverage.

### Known gaps

- No controller/integration/HTTP-level tests, so endpoint → status-code mapping is not directly verified by automated tests.
- No tests for the detailed user-deletion cascade behavior (authored recipes, other users' favorites).
- No tests for reference-data `get-or-create` behavior.
- No frontend tests.

See `docs/completion-checklist.md` for a manual verification checklist covering these gaps.

---

## 10. Running locally

Detailed instructions are in `docs/backend-runbook.md` and `docs/frontend-runbook.md`. Summary:

**Prerequisites:** .NET SDK 10.0, Node.js ≥ 18 + npm ≥ 8, PostgreSQL (recent/current) running on `localhost:5432`, and the `dotnet-ef` tool.

**Configuration:** copy `backend/RecipeManager.Api/appsettings.Development.json.example` to `appsettings.Development.json` and set your PostgreSQL connection string and a JWT signing key (≥ 32 characters). The `.example` file contains placeholders only; the real `appsettings.Development.json` is ignored by Git.

**Database:** create the database and apply existing migrations:

```bash
cd backend
dotnet ef database update --project RecipeManager.Infrastructure --startup-project RecipeManager.Api
```

Migrations seed roles and reference data. Users are not seeded — create an account through the public registration flow.

**Backend:** the API starts at `http://localhost:5053` (HTTP) / `https://localhost:7151` (HTTPS). Swagger UI is available at `/swagger` in Development.

```bash
cd backend
dotnet run --project RecipeManager.Api
```

**Frontend:** the Angular dev server starts at `http://localhost:4200` and is configured to call the API at `http://localhost:5053`.

```bash
cd frontend
npm install
ng serve
```

---

## 11. Documentation

The `docs/` directory holds focused reference material:

- `docs/api.md` — complete REST API contract (endpoints, request/response examples).
- `docs/auth-flow.md` — passwordless authentication design and implementation notes.
- `docs/domain_model.md` — entity model, relationships, and constraints.
- `docs/result-convention.md` — the `Result<T>` to HTTP status-code mapping.
- `docs/backend-runbook.md` and `docs/frontend-runbook.md` — local setup and development guides.
- `docs/scheme.sql` / `docs/drop.sql` — PostgreSQL DDL for reference (kept in sync with migrations).
- `docs/class-diagram.png`, `docs/er-diagram.png` — class and entity-relationship diagrams.

---

## 12. Known limitations / deferred work

- **Frontend tests** — Jasmine/Karma are configured but no spec files exist. Deferred.
- **Admin user-management UI** — the backend and route exist; the frontend screen is a placeholder and in progress.
- **`returnUrl` is incomplete** — `authGuard` writes a `returnUrl` query parameter, but no component yet consumes it; after login the user is routed to the recipe area regardless of the originally requested path.
- **Token expiry / session UX** — the frontend does not yet globally detect an expired JWT (401) to redirect or refresh; components display local error messages. No refresh-token flow.
- **Rate limiting / attempt limits** — login-code requests and verification attempts are not rate-limited. Noted in `docs/auth-flow.md` as future work.
- **Email delivery** — the login code is logged to the console in development; there is no email provider.
- **Image upload/storage** — recipes store an image URL only; there is no upload subsystem.
- **No production infrastructure** — no Docker, CI/CD, or deployment configuration. Swagger is Development-only. CORS is configured for local development origins.
- **Search / filtering / pagination** — not implemented.

---

## 13. Roadmap

### Near-term quality

- Add frontend unit tests (Jasmine/Karma already configured).
- Complete `returnUrl` deep-link handling and add global expired-token (401) handling.
- Add controller/integration tests and deletion-cascade tests.

### Functional enhancements

- Admin user-management UI.
- Recipe search / filtering / sorting / pagination.

### Production-oriented

- Rate limiting on authentication endpoints and login-code attempt limits.
- Real email delivery for login codes.
- Refresh-token flow and stronger session storage (e.g. HttpOnly cookies).
- Containerization and CI/CD.

---

## 14. License / attribution

This repository is a personal educational/portfolio project. No license file is currently present in the repository.
