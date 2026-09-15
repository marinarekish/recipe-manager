# Recipe Manager — Documentation

> Index and reading guide. This folder is the single source of developer
> documentation for the current app (Angular 19 SPA + ASP.NET Core / .NET 10
> API + PostgreSQL).

## Start here

1. **`README.md`** (repo root) — what the app is, stack, features, architecture,
   auth, and known limitations.
2. **`runbooks/backend-runbook.md`** — set up PostgreSQL, connection string,
   migrations, and run the API.
3. **`runbooks/frontend-runbook.md`** — `ng serve`, `apiBaseUrl`, CORS origins,
   and how to read the login code from the API logs.
4. **`demo-script.md`** — portfolio video / walkthrough scenarios
   (`▶ VIDEO` markers, ~5–8 min).

## Map

| Path                                   | Contents                                                   |
| -------------------------------------- | ---------------------------------------------------------- |
| `README.md`                            | This index                                                 |
| `demo-script.md`                       | Local screen-recording scenarios for a portfolio demo     |
| `api/api.md`                           | REST API contract (endpoints, request/response examples)   |
| `auth/auth-flow.md`                    | Passwordless login design + implementation notes           |
| `architecture/domain_model.md`         | Entities, relationships, constraints, business rules       |
| `architecture/result-convention.md`    | `Result<T>` → HTTP status-code mapping                     |
| `runbooks/backend-runbook.md`          | Backend setup, migrations, conventions                     |
| `runbooks/frontend-runbook.md`         | Frontend setup, auth flow, API base URL, CORS              |
| `checklist/completion-checklist.md`    | Manual verification checklist (maintainer-facing)          |
| `reference/scheme.sql`                 | PostgreSQL DDL (kept in sync with EF migrations)           |
| `reference/drop.sql`                   | Tear-down script (mirrors `scheme.sql`)                    |
| `reference/er-diagram.png`             | Entity-relationship diagram                                |
| `reference/class-diagram.png`          | Class diagram                                              |

## Conventions

- Language: **English** (repo convention).
- API documentation is best-effort; the **source of truth is Swagger**
  (`/swagger` when running in Development) and the code. If `api/api.md`
  drifts, flag it and update rather than assuming it is current.
- SQL/diagram files under `reference/` are checked by maintainers before
  editing; do not delete them without checking who links to them.