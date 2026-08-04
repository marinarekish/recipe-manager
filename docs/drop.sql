-- foreign keys
ALTER TABLE recipes
    DROP CONSTRAINT Recipe_Category;

ALTER TABLE recipes
    DROP CONSTRAINT Recipe_Cuisine;

ALTER TABLE recipe_ingredients
    DROP CONSTRAINT Recipe_Ingredient_Ingredient;

ALTER TABLE recipe_ingredients
    DROP CONSTRAINT Recipe_Ingredient_Recipe;

ALTER TABLE recipes
    DROP CONSTRAINT Recipe_User;

ALTER TABLE user_favorites
    DROP CONSTRAINT User_Recipe_Recipe;

ALTER TABLE user_favorites
    DROP CONSTRAINT User_Recipe_User;

ALTER TABLE users_roles
    DROP CONSTRAINT User_Role_Role;

ALTER TABLE users_roles
    DROP CONSTRAINT User_Role_User;

-- tables
DROP TABLE categories;

DROP TABLE cuisines;

DROP TABLE ingredients;

DROP TABLE recipe_ingredients;

DROP TABLE recipes;

DROP TABLE roles;

DROP TABLE user_favorites;

DROP TABLE users;

DROP TABLE users_roles;

-- End of file.