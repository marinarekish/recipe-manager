-- Recipe Manager — drop script (mirrors docs/scheme.sql)
-- Drops foreign keys first, then tables in reverse dependency order.

-- foreign keys
ALTER TABLE login_tokens DROP CONSTRAINT IF EXISTS FK_login_tokens_users_user_id;

ALTER TABLE users_roles DROP CONSTRAINT IF EXISTS FK_users_roles_users_user_id;
ALTER TABLE users_roles DROP CONSTRAINT IF EXISTS FK_users_roles_roles_role_id;

ALTER TABLE user_favorites DROP CONSTRAINT IF EXISTS FK_user_favorites_users_user_id;
ALTER TABLE user_favorites DROP CONSTRAINT IF EXISTS FK_user_favorites_recipes_recipe_id;

ALTER TABLE recipe_ingredients DROP CONSTRAINT IF EXISTS FK_recipe_ingredients_recipes_recipe_id;
ALTER TABLE recipe_ingredients DROP CONSTRAINT IF EXISTS FK_recipe_ingredients_ingredients_ingredient_id;

ALTER TABLE recipes DROP CONSTRAINT IF EXISTS FK_recipes_users_author_id;
ALTER TABLE recipes DROP CONSTRAINT IF EXISTS FK_recipes_categories_category_id;
ALTER TABLE recipes DROP CONSTRAINT IF EXISTS FK_recipes_cuisines_cuisine_id;

-- tables
DROP TABLE IF EXISTS recipe_ingredients;
DROP TABLE IF EXISTS user_favorites;
DROP TABLE IF EXISTS users_roles;
DROP TABLE IF EXISTS login_tokens;
DROP TABLE IF EXISTS recipes;
DROP TABLE IF EXISTS roles;
DROP TABLE IF EXISTS cuisines;
DROP TABLE IF EXISTS categories;
DROP TABLE IF EXISTS ingredients;
DROP TABLE IF EXISTS users;

-- End of file.
