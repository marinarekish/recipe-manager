# Backend — Local Setup & Development Runbook

How to get `RecipeManager.Api` running locally against PostgreSQL.

## Prerequisites

- .NET SDK **10.0** (`dotnet --version`)
- PostgreSQL running on `localhost:5432` (any recent version)
- `dotnet-ef` CLI tool (for migrations):

```bash
dotnet tool install --global dotnet-ef
```

## Project layout

```
backend/
├── RecipeManager.slnx              # solution (new XML format)
├── RecipeManager.Api               # ASP.NET Core host + controllers (net10.0)
├── RecipeManager.Application       # services, interfaces, DTOs, AutoMapper
├── RecipeManager.Domain            # entities (no dependencies)
├── RecipeManager.Infrastructure    # EF Core DbContext, configs, migrations
└── RecipeManager.Tests             # test project (not yet wired to the app)
```

Dependency direction: `Api → Application → (Domain, Infrastructure)`.
`Application` intentionally references `Infrastructure` and uses
`ApplicationDbContext` directly — there is no repository layer.

## 1. Configure the connection string

`appsettings.json` leaves the connection string empty. Set it in
Development via `appsettings.Development.json` (already populated for a
local install) or user-secrets:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
  "Host=localhost;Port=5432;Database=recipe_manager;Username=postgres;Password=postgres"
```

The connection is resolved in `Program.cs` as
`GetConnectionString("DefaultConnection")`.

## 2. Create the database and apply migrations

```bash
cd backend
dotnet ef database update \
  --project RecipeManager.Infrastructure \
  --startup-project RecipeManager.Api
```

This applies the existing migrations:

1. `20260805135746_InitialCreate`
2. `20260806104603_AddLoginTokens`
3. `20260810120655_FixPendingModelChanges`
4. `20260813075849_WidenIngredientAmount`

Migrations seed lookup data automatically: categories, cuisines,
ingredients, and roles (`Administrator`, `User`).

> **Note:** users are **not** seeded. Insert a user manually for recipe
> CRUD to work, e.g.:
>
> ```sql
> INSERT INTO users (first_name, last_name, email, created_at, updated_at)
> VALUES ('Admin', 'Admin', 'admin@example.com', now(), now());
> -- role assignment (optional)
> INSERT INTO users_roles (user_id, role_id) VALUES (1, 1);
> ```

## 3. Run the API

```bash
cd backend
dotnet run --project RecipeManager.Api
```

- HTTP: `http://localhost:5053`
- HTTPS: `https://localhost:7151`
- Swagger UI (Development only): `/swagger`

## CORS

The API uses a named CORS policy (`"FrontendDevelopment"`) configured via
the `Cors` section in `appsettings.json` / `appsettings.Development.json`:

```json
{
  "Cors": {
    "Origins": [
      "http://localhost:5173",
      "http://localhost:4200"
    ]
  }
}
```

Allowed origins, headers (`Authorization`, `Content-Type`), and methods
(`GET`, `POST`, `PUT`, `DELETE`, `OPTIONS`) are all config-driven.
The CORS policy is applied before authentication in the middleware pipeline
so preflight `OPTIONS` requests succeed without a token.

> **Note:** The `appsettings.Development.json` file is git-ignored.
> Copy `appsettings.Development.json.example` if it does not exist.
> The origins above cover Vite (`:5173`) and Angular (`:4200`) dev servers.

## Migrations workflow

After changing entity/config files:

```bash
# add a new migration
dotnet ef migrations add <Name> \
  --project RecipeManager.Infrastructure \
  --startup-project RecipeManager.Api

# review generated SQL
dotnet ef migrations script --idempotent \
  --project RecipeManager.Infrastructure \
  --startup-project RecipeManager.Api

# apply
dotnet ef database update \
  --project RecipeManager.Infrastructure \
  --startup-project RecipeManager.Api

# remove the last, not-yet-applied migration
dotnet ef migrations remove \
  --project RecipeManager.Infrastructure \
  --startup-project RecipeManager.Api
```

### Guard against model drift

The `FixPendingModelChanges` migration was needed because the model had
drifted from the migrations. Check for pending changes before committing:

```bash
dotnet ef migrations has-pending-model-changes \
  --project RecipeManager.Infrastructure \
  --startup-project RecipeManager.Api
```

Exit code is non-zero when the model differs from the snapshot. Add this to
CI so drift cannot slip in silently.

## Conventions

- **Result pattern** — service methods return `Result` / `Result<T>`
  instead of throwing exceptions or returning `null`. Controllers map
  these to HTTP via `ToActionResult()` — no per-exception `try/catch`
  blocks. See `result-convention.md`.
- **JWT Bearer auth** — endpoints require a valid JWT token except
  `/api/auth/*`. Identity comes from JWT claims (`sub` = userId,
  `role` = role). Admin-only endpoints use
  `[Authorize(Roles = "Administrator")]`.
- **EF Configurations** — one `IEntityTypeConfiguration<T>` per entity in
  `RecipeManager.Infrastructure/Configurations`, registered via
  `ApplyConfigurationsFromAssembly` in `ApplicationDbContext`.
- **DTOs** — contracts live in `RecipeManager.Application/Contracts`;
  responses are classes with `init`-only properties (designed for
  `ProjectTo<T>`). Requests are records.
- **Mapping** — AutoMapper profiles in
  `RecipeManager.Application/Mapping`; services project entities to DTOs
  with `ProjectTo` and never return entities.
- **camelCase JSON** — configured via `JsonNamingPolicy.CamelCase` in
  `Program.cs`. C# DTO properties remain PascalCase; the serializer
  handles the conversion.
- **Timestamps** — `created_at`/`updated_at` are `timestamptz` with DB
  defaults; services also set `DateTime.UtcNow` explicitly.
- **Naming** — tables/columns are `snake_case`, entities are `PascalCase`
  with explicit `HasColumnName`/`ToTable`.
