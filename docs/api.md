# Recipe Manager — REST API Contract

> Actual state as of the RecipeController PR. Auth is stubbed with constants
> (see "Authorization" below); JWT is out of scope for now.

All endpoints are served under `/api` by the `RecipeManager.Api` project.
JSON is used for request/response bodies. Property names are serialized in
PascalCase by default.

---

## Authorization (current stub)

Until JWT lands, `RecipeController` uses two compile-time constants:

```csharp
private const int CurrentUserId = 1; // admin user id
private const bool IsAdmin = true;   // true = Admin, false = User
```

Consequences of the stub:

- "Current user" is always user id `1` — **that user must exist in the
  database** or `GET /api/recipes/me` and `POST /api/recipes` return `404`.
- `GET /api/recipes` (admin-only) returns `403` when `IsAdmin` is `false`.
  When `IsAdmin` is `true` the `403` branch is compiled out entirely
  (compiler warning CS0162).
- There is no `[Authorize]` yet; every request is treated as authenticated.

---

## Categories — `/api/categories`

Lookup data; update/delete intentionally out of scope.

### `GET /api/categories`
Returns all categories.

- **200** — `List<CategoryResponse>`

### `GET /api/categories/{id}`
- **200** — `CategoryResponse`
- **404** — not found

### `POST /api/categories/get-or-create`
Creates a category if it does not exist, otherwise returns the existing one
(matched case-insensitively). Idempotent.

Request:

```json
{ "name": "Dessert" }
```

- **200** — `CategoryResponse`
- **400** — empty name (`{ "message": "..." }`)

`CategoryResponse`: `{ "categoryId": 1, "name": "Dessert" }`

---

## Cuisines — `/api/cuisines`

Same contract as categories:

### `GET /api/cuisines` → `List<CuisineResponse>` (200)
### `GET /api/cuisines/{id}` → `CuisineResponse` (200) | 404
### `POST /api/cuisines/get-or-create` → `CuisineResponse` (200) | 400

`CuisineResponse`: `{ "cuisineId": 1, "name": "Italian" }`

---

## Ingredients — `/api/ingredients`

### `GET /api/ingredients` → `List<IngredientResponse>` (200)
### `GET /api/ingredients/{id}` → `IngredientResponse` (200) | 404
### `POST /api/ingredients/get-or-create` → `IngredientResponse` (200) | 400

`IngredientResponse`: `{ "ingredientId": 1, "name": "Milk" }`

---

## Recipes — `/api/recipes`

### `GET /api/recipes`
Admin only. Returns every recipe.

- **200** — `List<RecipeResponse>`
- **403** — non-admin

### `GET /api/recipes/me`
Returns the current user's recipes.

- **200** — `List<RecipeResponse>`
- **404** — current user does not exist in DB

### `GET /api/recipes/{id}`  (`id` must be an integer)
- **200** — `RecipeResponse`
- **403** — recipe exists but belongs to another user (non-admin)
- **404** — not found

### `POST /api/recipes`
Creates a recipe owned by the current user. Cuisine, category and
ingredients accept **either an id or a name** (id-or-name fallback):

- a provided valid `*Id` is used as-is;
- otherwise the `*Name` is resolved via get-or-create (unknown names are
  created, matched case-insensitively);
- if neither is provided, `400` is returned.

Request (`CreateRecipeRequest`):

```json
{
  "title": "Tomato Soup",
  "cuisineId": 2,
  "categoryName": "Main Course",
  "prepTimeMinutes": 10,
  "cookTimeMinutes": 30,
  "servings": 4,
  "instructions": "Cook everything.",
  "ingredients": [
    { "name": "Tomato", "amount": 500, "unit": "g" },
    { "ingredientId": 3, "amount": 5, "unit": "g" }
  ]
}
```

Validation: `title` required; `ingredients` required (≥ 1 item);
`*Id` (when present) ≥ 1; `prepTimeMinutes`/`cookTimeMinutes`/`servings`
≥ 1; ingredient `amount` in `[0.01, 99999999.99]`.

- **201** — `RecipeResponse` with `Location` header
  (`GET /api/recipes/{id}`)
- **404** — current user does not exist
- **400** — validation/model-binding failure, or a cuisine/category/ingredient
  missing both id and name

### `PUT /api/recipes/{id}`  (idempotent full/partial update)
Request (`UpdateRecipeRequest`) — every field optional; omitted/null fields
keep their current value. Cuisine/category/ingredients use the same
id-or-name fallback as `POST` when provided:

```json
{
  "title": "Tomato Basil Soup",
  "cuisineId": 2,
  "ingredients": [
    { "name": "Tomato",   "amount": 600, "unit": "g" },
    { "ingredientId": 7, "amount": 20,  "unit": "ml" }
  ]
}
```

- **200** — `RecipeResponse`
- **403** — recipe exists but belongs to another user (non-admin)
- **404** — not found
- **400** — validation/model-binding failure, or a cuisine/category/ingredient
  missing both id and name

### `DELETE /api/recipes/{id}`
- **204** — deleted
- **403** — recipe exists but belongs to another user (non-admin)
- **404** — not found

---

## Response shapes

`RecipeResponse`:

```json
{
  "recipeId": 1,
  "title": "Tomato Soup",
  "prepTimeMinutes": 10,
  "cookTimeMinutes": 30,
  "servings": 4,
  "instructions": "Cook everything.",
  "cuisineId": 1,
  "cuisineName": "Italian",
  "categoryId": 8,
  "categoryName": "Main Course",
  "authorId": 1,
  "authorName": "Maryna Rekish",
  "ingredients": [
    { "ingredientId": 11, "name": "Tomato", "amount": 500, "unit": "g" }
  ],
  "createdAt": "2026-08-11T15:00:00Z",
  "updatedAt": "2026-08-11T15:00:00Z"
}
```

Timestamps are UTC (`DateTime.UtcNow`); DB columns are `timestamptz`.

---

## Not yet exposed via HTTP

- **Auth** — `IAuthService`/`AuthService` are implemented but there is no
  `AuthController`, so no login endpoints exist yet. See `auth-flow.md`.
- **Users** — `IUserService` is stubbed (`NotImplementedException`); no
  `UserController`.
- **Favorites** — `IFavoriteService` is implemented; no `FavoriteController`.

Swagger UI (`/swagger`) is enabled in Development only.
