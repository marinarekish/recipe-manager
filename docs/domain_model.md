# Recipe Manager — Domain Model

## Overview

Recipe Manager is a full-stack web application for storing, organizing, and browsing cooking recipes.

The application allows users to create recipes, categorize them by cuisine and category, manage ingredients, and save favorite recipes for quick access.

The primary goal of the project is to serve as a learning platform for modern full-stack development using ASP.NET Core, Entity Framework Core, PostgreSQL, React, and TypeScript while following clean architectural principles.

---

# Domain Entities

## User

Represents an application user.

Responsibilities:

- Create recipes
- Save favorite recipes
- Have one or more roles

### Attributes

| Attribute  | Description           |
| ---------- | --------------------- |
| user_id    | Primary key           |
| first_name | User first name       |
| last_name  | User last name        |
| email      | Unique email          |
| phone      | Optional phone number |
| created_at | Creation timestamp    |
| updated_at | Last update timestamp |

---

## Role

Represents a user role within the application.

Responsibilities:

- Authorization
- Permission grouping

Examples:

- User
- Admin

### Attributes

| Attribute | Description |
| --------- | ----------- |
| role_id   | Primary key |
| name      | Role name   |

---

## Recipe

Represents a cooking recipe.

Responsibilities:

- Store recipe metadata
- Reference author
- Reference category
- Reference cuisine
- Store cooking instructions

### Attributes

| Attribute         | Description           |
| ----------------- | --------------------- |
| recipe_id         | Primary key           |
| author_id         | Recipe author         |
| category_id       | Recipe category       |
| cuisine_id        | Recipe cuisine        |
| title             | Recipe title          |
| prep_time_minutes | Preparation time      |
| cook_time_minutes | Cooking time          |
| servings          | Number of servings    |
| instructions      | Cooking instructions  |
| created_at        | Creation timestamp    |
| updated_at        | Last update timestamp |

---

## Category

Represents a recipe category.

Examples:

- Dessert
- Breakfast
- Soup
- Salad

### Attributes

| Attribute   | Description   |
| ----------- | ------------- |
| category_id | Primary key   |
| name        | Category name |

---

## Cuisine

Represents a cuisine.

Examples:

- Italian
- Japanese
- Mexican

### Attributes

| Attribute  | Description  |
| ---------- | ------------ |
| cuisine_id | Primary key  |
| name       | Cuisine name |

---

## Ingredient

Represents an ingredient.

Examples:

- Tomato
- Salt
- Milk

### Attributes

| Attribute     | Description     |
| ------------- | --------------- |
| ingredient_id | Primary key     |
| name          | Ingredient name |

---

## RecipeIngredient

Represents the many-to-many relationship between recipes and ingredients.

Responsibilities:

- Store ingredient quantity
- Store measurement unit

### Attributes

| Attribute     | Description      |
| ------------- | ---------------- |
| recipe_id     | Recipe           |
| ingredient_id | Ingredient       |
| amount        | Quantity         |
| unit          | Measurement unit |

---

## UserFavorite

Represents recipes saved by users.

Responsibilities:

- Store user's favorite recipes

### Attributes

| Attribute  | Description           |
| ---------- | --------------------- |
| user_id    | User                  |
| recipe_id  | Favorite recipe       |
| created_at | Added timestamp       |
| updated_at | Last update timestamp |

---

## UserRole

Represents the many-to-many relationship between users and roles.

### Attributes

| Attribute | Description |
| --------- | ----------- |
| user_id   | User        |
| role_id   | Role        |

---

# Relationships

| Relationship          | Cardinality | Delete Behavior                         |
| --------------------- | ----------- | --------------------------------------- |
| User → Recipe         | 1:N         | Restrict                                |
| User ↔ Role           | M:N         | Restrict                                |
| User ↔ FavoriteRecipe | M:N         | Cascade                                 |
| Recipe → Category     | N:1         | Restrict                                |
| Recipe → Cuisine      | N:1         | Restrict                                |
| Recipe ↔ Ingredient   | M:N         | Cascade (Recipe), Restrict (Ingredient) |

---

# Business Scenarios

## Create Recipe

A user creates a recipe by providing:

- title
- category
- cuisine
- preparation time
- cooking time
- servings
- instructions
- ingredients

---

## Browse Recipes

Users can browse recipes by:

- category
- cuisine

Future versions may include:

- search
- sorting
- filtering

---

## Manage Favorites

Users can:

- add recipes to favorites
- remove recipes from favorites
- view favorite recipes

---

# Database Constraints

The schema includes the following constraints.

## Primary Keys

Every entity uses a surrogate primary key based on an identity column.

Junction tables use composite primary keys.

---

## Foreign Keys

Foreign keys maintain referential integrity between entities.

---

## UNIQUE Constraints

Applied to:

- users.email
- categories.name
- cuisines.name
- ingredients.name
- roles.name

---

## NOT NULL Constraints

Required business fields are marked as NOT NULL.

Optional fields:

- phone
- instructions

---

## CHECK Constraints

Validation rules include:

- amount > 0
- prep_time_minutes > 0
- cook_time_minutes > 0
- servings > 0

---

## Default Values

Timestamp fields are initialized using:

- CURRENT_TIMESTAMP

---

## Candidate Indexes

Future indexes:

- recipes(author_id)
- recipes(category_id)
- recipes(cuisine_id)
- recipe_ingredients(ingredient_id)
- user_favorites(recipe_id)

---

# Normalization

The database schema satisfies Third Normal Form (3NF).

Normalization decisions include:

- Categories extracted into a lookup table
- Cuisines extracted into a lookup table
- Ingredients stored independently
- Many-to-many relationships implemented using junction tables
- No duplicated business data

---

# Architectural Decisions

Several design decisions were made during the modeling process.

## Separate lookup tables

Categories, cuisines, and roles are stored independently to eliminate duplication and simplify future expansion.

## Junction tables

Many-to-many relationships are represented using dedicated junction tables:

- recipe_ingredients
- users_roles
- user_favorites

This design supports normalization and additional metadata where required.

## Recipe ownership

Each recipe has a single author referenced by `author_id`.

## Ingredient quantities

Ingredient quantities are stored in the junction table because they depend on a specific recipe.

---

# Future Extensions

Possible future improvements include:

- Recipe images
- Recipe comments
- Ratings and reviews
- Step-by-step cooking instructions
- Authentication using JWT
- Search by ingredient
- Pagination
- Nutritional information
- Shopping lists
