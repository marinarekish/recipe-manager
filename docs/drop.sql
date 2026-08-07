-- Created by Redgate Data Modeler (https://datamodeler.redgate-platform.com)
-- Last modification date: 2026-08-06 11:26:22.705

-- foreign keys
ALTER TABLE users_roles
    DROP CONSTRAINT User_Role_Role;

ALTER TABLE users_roles
    DROP CONSTRAINT User_Role_User;

ALTER TABLE recipe_ingredients
    DROP CONSTRAINT ingredients_recingredients;

ALTER TABLE login_tokens
    DROP CONSTRAINT login_users;

ALTER TABLE recipes
    DROP CONSTRAINT recipe_category;

ALTER TABLE recipes
    DROP CONSTRAINT recipe_cuisine;

ALTER TABLE recipe_ingredients
    DROP CONSTRAINT recipe_recingr;

ALTER TABLE recipes
    DROP CONSTRAINT recipe_user;

ALTER TABLE user_favorites
    DROP CONSTRAINT recipe_userfav;

ALTER TABLE user_favorites
    DROP CONSTRAINT user_userfav;

-- tables
DROP TABLE categories;

DROP TABLE cuisines;

DROP TABLE ingredients;

DROP TABLE login_tokens;

DROP TABLE recipe_ingredients;

DROP TABLE recipes;

DROP TABLE roles;

DROP TABLE user_favorites;

DROP TABLE users;

DROP TABLE users_roles;

-- End of file.

