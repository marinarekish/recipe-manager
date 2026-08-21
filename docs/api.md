# Recipe Manager — REST API Contract

All endpoints are served under `/api` by the `RecipeManager.Api` project.
JSON is used for request/response bodies. Property names use **camelCase**
(configured via `JsonNamingPolicy.CamelCase` in `Program.cs`). C# DTO
property names remain PascalCase; the serializer handles the conversion.

Error responses use the Result pattern — see `result-convention.md` for
the full mapping.

---

## Authorization

All endpoints require a valid JWT Bearer token except `/api/auth/*`.
Admin-only endpoints use `[Authorize(Roles = "Administrator")]`.

Obtain a token via `POST /api/auth/verify-code`, then pass it in the
`Authorization` header:

```
Authorization: Bearer {token}
```

User identity is derived from JWT claims (`sub` = userId, `role` = role).
No `[Authorize]` means anonymous access (auth endpoints only).

---

## Auth — `/api/auth`

Passwordless login via email + one-time code. See `auth-flow.md`.

### `POST /api/auth/request-code`

Issues a 6-digit login code for the given email. Returns the same
response whether the email exists or not (to prevent user enumeration
at the HTTP level — the service layer distinguishes internally).

Request:

```json
{ "email": "user@example.com" }
```

- **200** — `{ "message": "If the account exists, a login code has been issued." }`
- **404** — email not found

### `POST /api/auth/verify-code`

Verifies the code, returns the authenticated user profile and a JWT access
token.

Request:

```json
{ "email": "user@example.com", "code": "123456" }
```

- **200** — `AuthResponse` (includes access token)
- **401** — code invalid or expired
- **404** — email not found

`AuthResponse`:

```json
{
  "user": {
    "userId": 1,
    "firstName": "Maryna",
    "lastName": "Rekish",
    "email": "maryna@example.com",
    "phone": null,
    "roles": [{ "roleId": 1, "name": "Administrator" }],
    "createdAt": "2026-08-11T15:00:00Z"
  },
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "expiresIn": 3600
}
```

Use `accessToken` in the `Authorization: Bearer {token}` header for all
subsequent requests.

---

## Categories — `/api/categories`

Lookup data with admin-only delete.

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
- **400** — empty name (`{ "message": "...", "errors": ["..."] }`)

`CategoryResponse`: `{ "categoryId": 1, "name": "Dessert" }`

### `DELETE /api/categories/{id}`
Admin only. Deletes a category by id.

- **204** — deleted
- **401** — no token / invalid token
- **403** — non-admin
- **404** — not found
- **409** — category still in use by recipes

---

## Cuisines — `/api/cuisines`

### `GET /api/cuisines` → `List<CuisineResponse>` (200)
### `GET /api/cuisines/{id}` → `CuisineResponse` (200) | 404
### `POST /api/cuisines/get-or-create` → `CuisineResponse` (200) | 400

### `DELETE /api/cuisines/{id}`
Admin only. Deletes a cuisine by id.

- **204** — deleted
- **401** — no token / invalid token
- **403** — non-admin
- **404** — not found
- **409** — cuisine still in use by recipes

`CuisineResponse`: `{ "cuisineId": 1, "name": "Italian" }`

---

## Ingredients — `/api/ingredients`

### `GET /api/ingredients` → `List<IngredientResponse>` (200)
### `GET /api/ingredients/{id}` → `IngredientResponse` (200) | 404
### `POST /api/ingredients/get-or-create` → `IngredientResponse` (200) | 400

### `DELETE /api/ingredients/{id}`
Admin only. Deletes an ingredient by id.

- **204** — deleted
- **401** — no token / invalid token
- **403** — non-admin
- **404** — not found
- **409** — ingredient still in use by recipes

`IngredientResponse`: `{ "ingredientId": 1, "name": "Milk" }`

---

## Recipes — `/api/recipes`

### `GET /api/recipes`
Authenticated users only. Returns every recipe.

- **200** — `List<RecipeResponse>`
- **401** — no token / invalid token

### `GET /api/recipes/me`
Returns the current user's recipes.

- **200** — `List<RecipeResponse>`
- **401** — no token / invalid token
- **404** — current user does not exist in DB

### `GET /api/recipes/{id}`  (`id` must be an integer)
Any authenticated user may read any recipe.

- **200** — `RecipeResponse`
- **401** — no token / invalid token
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
  "imageUrl": "https://example.com/images/tomato-soup.jpg",
  "ingredients": [
    { "name": "Tomato", "amount": 500, "unit": "g" },
    { "ingredientId": 3, "amount": 5, "unit": "g" }
  ]
}
```

Validation: `title` required; `ingredients` required (≥ 1 item);
`*Id` (when present) ≥ 1; `prepTimeMinutes`/`cookTimeMinutes`/`servings`
≥ 1; ingredient `amount` in `[0.01, 99999999.99]`; `imageUrl` optional
(≤ 2048 chars, must contain non-whitespace when present). `imageUrl` is a
plain URL string (absolute or relative) for future hero-image display;
file upload/storage is out of scope — omitting it stores `null` and the
UI is expected to show a placeholder.

- **201** — `RecipeResponse` with `Location` header
  (`GET /api/recipes/{id}`)
- **401** — no token / invalid token
- **404** — current user does not exist
- **400** — validation failure, or a cuisine/category/ingredient
  missing both id and name

### `PUT /api/recipes/{id}`  (idempotent full/partial update)
Owner or Administrator only. Request (`UpdateRecipeRequest`) — every field
optional; omitted/null fields keep their current value. Cuisine/category/
ingredients use the same id-or-name fallback as `POST` when provided:

```json
{
  "title": "Tomato Basil Soup",
  "cuisineId": 2,
  "imageUrl": "https://example.com/images/tomato-basil-soup.jpg",
  "ingredients": [
    { "name": "Tomato",   "amount": 600, "unit": "g" },
    { "ingredientId": 7, "amount": 20,  "unit": "ml" }
  ]
}
```

`imageUrl` follows the same rule as every other field: omitted/null keeps
the current value (there is no way to clear it back to `null` yet).

- **200** — `RecipeResponse`
- **401** — no token / invalid token
- **403** — recipe exists but belongs to another user (non-admin)
- **404** — not found
- **400** — validation failure, or a cuisine/category/ingredient
  missing both id and name

### `DELETE /api/recipes/{id}`
Owner or Administrator only.

- **204** — deleted
- **401** — no token / invalid token
- **403** — recipe exists but belongs to another user (non-admin)
- **404** — not found

---

## Favorites — `/api/favorites`

### `GET /api/favorites`
Returns the current user's favorite recipes.

- **200** — `List<FavoriteRecipeResponse>`
- **401** — no token / invalid token

### `POST /api/favorites`
Adds a recipe to the current user's favorites.

Request:

```json
{ "recipeId": 4 }
```

- **201** — `FavoriteRecipeResponse`
- **401** — no token / invalid token
- **404** — recipe not found
- **409** — recipe already in favorites

### `DELETE /api/favorites/{id}`
Removes a recipe from the current user's favorites.

- **204** — removed
- **401** — no token / invalid token
- **404** — favorite not found

`FavoriteRecipeResponse`:

```json
{
  "recipeId": 4,
  "title": "Test pasta",
  "addedAt": "2026-08-13T13:36:04.888564Z"
}
```

---

## Users — `/api/users`

### `GET /api/users/me`
Returns the current user's profile (includes roles).

- **200** — `UserResponse`
- **401** — no token / invalid token
- **404** — current user does not exist in DB

### `PUT /api/users/me`
Updates the current user's own profile. Every field optional; omitted/null
fields keep their current value. `email` (when provided) is trimmed,
lower-cased and must not belong to another user.

Request (`UpdateUserRequest`):

```json
{
  "firstName": "Maryna",
  "email": "maryna@example.com"
}
```

- **200** — `UserResponse`
- **400** — email already used by another user
- **401** — no token / invalid token
- **404** — current user does not exist

### `GET /api/users`
Admin only. Lists all users.

- **200** — `List<UserResponse>`
- **401** — no token / invalid token
- **403** — non-admin

### `GET /api/users/{id}`
Admin only.

- **200** — `UserResponse`
- **401** — no token / invalid token
- **403** — non-admin
- **404** — not found

### `PUT /api/users/{id}/role`
Admin only. Replaces the user's roles with the single role from the body.
Guarded so the system always keeps at least one Administrator: removing the
Administrator role from the last admin is rejected.

Request (`AssignRoleRequest`):

```json
{ "roleId": 2 }
```

- **200** — `UserResponse`
- **400** — would remove the role from the last administrator
- **401** — no token / invalid token
- **403** — non-admin
- **404** — user or role not found

`UserResponse`:

```json
{
  "userId": 1,
  "firstName": "Maryna",
  "lastName": "Rekish",
  "email": "maryna@example.com",
  "phone": null,
  "roles": [
    { "roleId": 1, "name": "Administrator" }
  ],
  "createdAt": "2026-08-11T15:00:00Z",
  "updatedAt": "2026-08-11T15:00:00Z"
}
```

New users (via `IUserService.CreateUserAsync`) get the `User` role by default.

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
  "imageUrl": null,
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

`imageUrl` is `null` unless a URL string was set via `POST`/`PUT`;
clients should fall back to a placeholder image when it is `null`.

---

## Error response format

All error responses follow the Result pattern (`result-convention.md`):

| Status | Body |
| ------ | ---- |
| 400 | `{ "message": "...", "errors": ["..."] }` |
| 401 | no body (RFC 9110 problem details) |
| 403 | no body |
| 404 | `{ "message": "..." }` or RFC 9110 problem details |
| 409 | `{ "message": "..." }` |

Swagger UI (`/swagger`) is enabled in Development only.
