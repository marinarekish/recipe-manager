# Frontend — Local Setup & Development Runbook

How to get the Angular SPA running locally against the backend API,
including database, auth, and data prerequisites.

## Ports & URLs

| Service     | URL                             | Notes                            |
| ----------- | ------------------------------- | -------------------------------- |
| PostgreSQL  | `localhost:5432`                | Default port                     |
| API         | `http://localhost:5053`         | HTTP (see `launchSettings.json`) |
| API (HTTPS) | `https://localhost:7151`        | Optional                         |
| Swagger     | `http://localhost:5053/swagger` | Development only                 |
| Frontend    | `http://localhost:4200`         | Angular dev server (`ng serve`)  |

CORS is configured for the Angular dev origin `http://localhost:4200`.
See the [CORS section](#cors) below.

## Prerequisites

| Requirement | Version | Check                                    |
| ----------- | ------- | ---------------------------------------- |
| .NET SDK    | 10.0    | `dotnet --version`                       |
| Node.js     | ≥ 18    | `node --version`                         |
| npm         | ≥ 8     | `npm --version`                          |
| PostgreSQL  | recent  | Running on `localhost:5432`              |
| `dotnet-ef` | latest  | `dotnet tool install --global dotnet-ef` |

## How to run

Start order: **database → API → frontend**.

### 1. Database

Ensure PostgreSQL is running on `localhost:5432`. Create the database
and apply migrations (see `docs/backend-runbook.md` for full details):

```bash
cd backend
dotnet ef database update \
  --project RecipeManager.Infrastructure \
  --startup-project RecipeManager.Api
```

### 2. Create a user (register)

There is public self-registration via `POST /api/auth/register`, so you no
longer need a manual SQL seed for a normal login. Register any new email:

```bash
curl -X POST http://localhost:5053/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"firstName":"Admin","lastName":"Admin","email":"admin@example.com","phone":null}'
```

Response (201) — user created with the default `User` role (no token
returned):

```json
{
  "userId": 1,
  "firstName": "Admin",
  "lastName": "Admin",
  "email": "admin@example.com",
  "phone": null,
  "roles": [{ "roleId": 2, "name": "User" }],
  "createdAt": "...",
  "updatedAt": "..."
}
```

To grant the Administrator role (optional), insert directly:

```sql
INSERT INTO users_roles (user_id, role_id) SELECT 1, 1
WHERE NOT EXISTS (SELECT 1 FROM users_roles WHERE user_id = 1 AND role_id = 1);
```

You can use any email — you will read the login code from the API logs,
not from an inbox.

### 3. Start the API

```bash
cd backend
dotnet run --project RecipeManager.Api
```

The API starts at `http://localhost:5053`. Swagger UI is available at
`/swagger` in Development.

### 4. Start the frontend

```bash
cd frontend
npm install   # first time only
ng serve
```

Opens at **http://localhost:4200**.

## API base URL configuration

The backend URL is configured in Angular environment files (not `.env`):

```
frontend/src/environments/
├── environment.ts                  # production  → http://localhost:5053
└── environment.development.ts      # development → http://localhost:5053
```

Angular CLI swaps these automatically via `fileReplacements` in
`angular.json`:

- `ng serve` uses `environment.development.ts`
- `ng build` uses `environment.ts`

Import and use in services:

```typescript
import { environment } from "../../environments/environment";

const url = `${environment.apiBaseUrl}/api/recipes`;
```

> **Do not** hardcode the API URL in components or services.

## Authentication (passwordless login)

There is no password-based login. Users authenticate with an email

- one-time 6-digit code. See `docs/auth-flow.md` for the full
  design.

### Step 1 — Request a code

```bash
curl -X POST http://localhost:5053/api/auth/request-code \
  -H "Content-Type: application/json" \
  -d '{"email": "admin@example.com"}'
```

Response (200):

```json
{ "message": "If the account exists, a login code has been issued." }
```

### Step 2 — Read the code from API logs

There is no email sender yet. The code is printed in the API's
console output:

```
Login code for user@example.com: 482916
```

### Step 3 — Verify the code

```bash
curl -X POST http://localhost:5053/api/auth/verify-code \
  -H "Content-Type: application/json" \
  -d '{"email": "admin@example.com", "code": "482916"}'
```

Response (200):

```json
{
  "user": {
    "userId": 1,
    "firstName": "Admin",
    "lastName": "Admin",
    "email": "admin@example.com",
    "phone": null,
    "roles": [{ "roleId": 1, "name": "Administrator" }],
    "createdAt": "2026-08-11T15:00:00Z"
  },
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "expiresIn": 3600
}
```

### Step 4 — Use the token

Send the `accessToken` on subsequent requests:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

Codes expire after 10 minutes. Requesting a new code invalidates
all previous active codes for that user.

## Data prerequisites

| Requirement                           | How to satisfy                                           |
| ------------------------------------- | -------------------------------------------------------- |
| PostgreSQL running                    | Start PostgreSQL on `localhost:5432`                     |
| Database created + migrations applied | `dotnet ef database update` (see above)                  |
| At least one user in DB               | `POST /api/auth/register` (see above)                    |
| JWT key configured                    | `Jwt:Key` in `appsettings.Development.json` (≥ 32 chars) |

### JWT settings

JWT configuration lives in `appsettings.Development.json` (git-ignored).
Copy the example if it does not exist:

```bash
cp backend/RecipeManager.Api/appsettings.Development.json.example \
   backend/RecipeManager.Api/appsettings.Development.json
```

Then edit `Jwt:Key` to a random string of at least 32 characters.
The `Issuer`, `Audience`, and `ExpirationMinutes` defaults are fine
for local development.

> **Never commit `appsettings.Development.json`** — it contains
> secrets. The `.example` file has placeholder values only.

## Contract notes

- **JSON property names are camelCase.** C# DTO properties are
  PascalCase; the serializer converts automatically
  (`JsonNamingPolicy.CamelCase` in `Program.cs`).
- Error responses use the Result pattern — see `docs/result-convention.md`.
- Full API reference: **`docs/api.md`**
- Auth flow details: **`docs/auth-flow.md`**

## CORS

The backend CORS policy (`"FrontendDevelopment"`) allows
`http://localhost:4200` (Angular dev server). Origins are configured in the
`Cors:Origins` array in `appsettings.Development.json`
(see `docs/backend-runbook.md`).

Allowed headers: `Authorization`, `Content-Type`.
Allowed methods: `GET`, `POST`, `PUT`, `DELETE`, `OPTIONS`.

If you change the Angular dev server port, add the new origin to that
array.

## Folder structure (current)

```
frontend/src/
├── app/
│   ├── core/
│   │   ├── auth/               # AuthService, auth models, authGuard, adminGuard, interceptor
│   │   └── recipes/            # recipeIdGuard (route param validation)
│   ├── shared/
│   │   └── components/         # recipe-card, recipe-grid, placeholder
│   ├── features/
│   │   ├── auth/               # login, register, verify-code pages
│   │   ├── layout/             # app shell (sidebar + router outlet)
│   │   ├── recipes/            # data/ (service + models) + pages/ (explore, detail, form, my-recipes)
│   │   ├── favorites/          # data/ + pages/ (favorites-list)
│   │   └── profile/            # data/ + pages/ (profile)
│   ├── app.component.ts / .html / .scss
│   ├── app.config.ts           # provideRouter, provideHttpClient + interceptor
│   └── app.routes.ts           # route definitions + guards
├── environments/               # environment.ts / environment.development.ts
├── index.html
├── main.ts                     # standalone bootstrap
└── styles.scss                 # global styles (Roboto, body reset)
```

Feature modules follow a `data/` (API services + models) and `pages/`
(view components) split. `core/` holds cross-cutting auth, `shared/`
holds reusable presentational components.

## Stack (current)

- **Angular 19** (standalone application, no NgModules)
- **TypeScript 5.7** (strict mode)
- **Angular Router** (`provideRouter`)
- **Angular Material + CDK** (azure-blue prebuilt theme)
- **RxJS 7.8** (ships with Angular)
- **SCSS** as the style language
- **`provideHttpClient(withInterceptors([authInterceptor]))`** wired in
  `app.config.ts` — the interceptor attaches the stored JWT to each request

## Implemented frontend behavior

- **Authentication UI** — register, login (request code), and verify-code
  pages. The register page chains registration with a login-code request and
  routes to the verify page. Codes are entered on `/verify?email=...`.
- **Session state** — `AuthService` keeps the current user in an Angular
  signal and persists the JWT + user profile in `localStorage`
  (`rm_access_token`, `rm_user`); the session is restored on reload.
- **Guards** — `authGuard` protects the application layout and redirects
  anonymous users to `/login`; `adminGuard` restricts the admin route to
  users with the `Administrator` role; `recipeIdGuard` validates the
  `:id` route parameter.
- **Routes** — protected area: `/recipes` (Explore), `/recipes/me`
  (My Recipes), `/recipes/new` (create), `/recipes/:id`,
  `/recipes/:id/edit`, `/favorites`, `/profile`, `/admin/users`
  (placeholder). Public: `/login`, `/register`, `/verify`.
- **Recipe screens** — explore list, my-recipes list, detail page, and a
  create/edit form with dynamic ingredient rows.
- **Favorites** — favorites list and heart toggle on recipe cards/detail.
- **Profile** — view/update the current user and delete their own account.

## What is NOT implemented yet

- **Frontend unit / e2e tests** — Jasmine + Karma are configured
  (`ng test`), but no spec files exist yet. Deferred.
- **Admin user-management UI** — the route exists but renders a
  placeholder component.
- **NgRx state management** — auth state uses Angular signals.
- **SSR / PWA**.
- **`returnUrl` deep-links** — `authGuard` writes a `returnUrl` query
  parameter, but no page consumes it yet; after login the user is routed
  to the recipes area.
- **Global expired-JWT (401) handling** — components show local error
  messages; there is no automatic redirect or refresh flow.

## Further reading

| Document                    | Contents                               |
| --------------------------- | -------------------------------------- |
| `docs/backend-runbook.md`   | Backend setup, migrations, conventions |
| `docs/api.md`               | Full REST API contract                 |
| `docs/auth-flow.md`         | Passwordless login design              |
| `docs/result-convention.md` | Result pattern → HTTP mapping          |
| `docs/domain_model.md`      | Entity relationships                   |
