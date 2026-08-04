-- tables
-- Table: categories
CREATE TABLE categories (
    category_id int  NOT NULL GENERATED ALWAYS AS IDENTITY,
    name varchar(50)  NOT NULL,
    CONSTRAINT uq_categories_name UNIQUE (name) NOT DEFERRABLE  INITIALLY IMMEDIATE,
    CONSTRAINT categories_pk PRIMARY KEY (category_id)
);

-- Table: cuisines
CREATE TABLE cuisines (
    cuisine_id int  NOT NULL GENERATED ALWAYS AS IDENTITY,
    name varchar(50)  NOT NULL,
    CONSTRAINT uq_cuisine_name UNIQUE (name) NOT DEFERRABLE  INITIALLY IMMEDIATE,
    CONSTRAINT cuisines_pk PRIMARY KEY (cuisine_id)
);

-- Table: ingredients
CREATE TABLE ingredients (
    ingredient_id int  NOT NULL GENERATED ALWAYS AS IDENTITY,
    name varchar(50)  NOT NULL,
    CONSTRAINT uq_ingredient_name UNIQUE (name) NOT DEFERRABLE  INITIALLY IMMEDIATE,
    CONSTRAINT ingredients_pk PRIMARY KEY (ingredient_id)
);

-- Table: recipe_ingredients
CREATE TABLE recipe_ingredients (
    recipe_id int  NOT NULL,
    ingredient_id int  NOT NULL,
    amount decimal(5,2)  NOT NULL,
    unit varchar(10)  NOT NULL,
    CONSTRAINT check_amount CHECK (amount > 0) NOT DEFERRABLE INITIALLY IMMEDIATE,
    CONSTRAINT recipe_ingredients_pk PRIMARY KEY (recipe_id,ingredient_id)
);

-- Table: recipes
CREATE TABLE recipes (
    recipe_id int  NOT NULL GENERATED ALWAYS AS IDENTITY,
    author_id int  NOT NULL,
    cuisine_id int  NOT NULL,
    category_id int  NOT NULL,
    title varchar(100)  NOT NULL,
    prep_time_minutes int  NOT NULL,
    cook_time_minutes int  NOT NULL,
    servings int  NOT NULL,
    instructions text  NULL,
    created_at timestamp  NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp  NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT check_prep_time CHECK (prep_time_minutes > 0) NOT DEFERRABLE INITIALLY IMMEDIATE,
    CONSTRAINT check_cook_time CHECK (cook_time_minutes > 0) NOT DEFERRABLE INITIALLY IMMEDIATE,
    CONSTRAINT check_servings CHECK (servings > 0) NOT DEFERRABLE INITIALLY IMMEDIATE,
    CONSTRAINT recipes_pk PRIMARY KEY (recipe_id)
);

-- Table: roles
CREATE TABLE roles (
    role_id int  NOT NULL GENERATED ALWAYS AS IDENTITY,
    name varchar(50)  NOT NULL,
    CONSTRAINT uq_roles_name UNIQUE (name) NOT DEFERRABLE  INITIALLY IMMEDIATE,
    CONSTRAINT roles_pk PRIMARY KEY (role_id)
);

-- Table: user_favorites
CREATE TABLE user_favorites (
    user_id int  NOT NULL,
    recipe_id int  NOT NULL,
    created_at timestamp  NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp  NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT user_favorites_pk PRIMARY KEY (user_id,recipe_id)
);

-- Table: users
CREATE TABLE users (
    user_id int  NOT NULL GENERATED ALWAYS AS IDENTITY,
    first_name varchar(50)  NOT NULL,
    last_name varchar(50)  NOT NULL,
    email varchar(255)  NOT NULL,
    phone varchar(20)  NULL,
    created_at timestamp  NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp  NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT uq_users_email UNIQUE (email) NOT DEFERRABLE  INITIALLY IMMEDIATE,
    CONSTRAINT users_pk PRIMARY KEY (user_id)
);

-- Table: users_roles
CREATE TABLE users_roles (
    user_id int  NOT NULL,
    role_id int  NOT NULL,
    CONSTRAINT users_roles_pk PRIMARY KEY (user_id,role_id)
);

-- foreign keys
-- Reference: Recipe_Category (table: recipes)
ALTER TABLE recipes ADD CONSTRAINT Recipe_Category
    FOREIGN KEY (category_id)
    REFERENCES categories (category_id)
    ON DELETE  RESTRICT  
    NOT DEFERRABLE 
    INITIALLY IMMEDIATE
;

-- Reference: Recipe_Cuisine (table: recipes)
ALTER TABLE recipes ADD CONSTRAINT Recipe_Cuisine
    FOREIGN KEY (cuisine_id)
    REFERENCES cuisines (cuisine_id)
    ON DELETE  RESTRICT  
    NOT DEFERRABLE 
    INITIALLY IMMEDIATE
;

-- Reference: Recipe_Ingredient_Ingredient (table: recipe_ingredients)
ALTER TABLE recipe_ingredients ADD CONSTRAINT Recipe_Ingredient_Ingredient
    FOREIGN KEY (ingredient_id)
    REFERENCES ingredients (ingredient_id)
    ON DELETE  RESTRICT  
    NOT DEFERRABLE 
    INITIALLY IMMEDIATE
;

-- Reference: Recipe_Ingredient_Recipe (table: recipe_ingredients)
ALTER TABLE recipe_ingredients ADD CONSTRAINT Recipe_Ingredient_Recipe
    FOREIGN KEY (recipe_id)
    REFERENCES recipes (recipe_id)
    ON DELETE  CASCADE  
    NOT DEFERRABLE 
    INITIALLY IMMEDIATE
;

-- Reference: Recipe_User (table: recipes)
ALTER TABLE recipes ADD CONSTRAINT Recipe_User
    FOREIGN KEY (author_id)
    REFERENCES users (user_id)
    ON DELETE  RESTRICT  
    NOT DEFERRABLE 
    INITIALLY IMMEDIATE
;

-- Reference: User_Recipe_Recipe (table: user_favorites)
ALTER TABLE user_favorites ADD CONSTRAINT User_Recipe_Recipe
    FOREIGN KEY (recipe_id)
    REFERENCES recipes (recipe_id)
    ON DELETE  CASCADE  
    NOT DEFERRABLE 
    INITIALLY IMMEDIATE
;

-- Reference: User_Recipe_User (table: user_favorites)
ALTER TABLE user_favorites ADD CONSTRAINT User_Recipe_User
    FOREIGN KEY (user_id)
    REFERENCES users (user_id)
    ON DELETE  CASCADE  
    NOT DEFERRABLE 
    INITIALLY IMMEDIATE
;

-- Reference: User_Role_Role (table: users_roles)
ALTER TABLE users_roles ADD CONSTRAINT User_Role_Role
    FOREIGN KEY (role_id)
    REFERENCES roles (role_id)
    ON DELETE  RESTRICT  
    NOT DEFERRABLE 
    INITIALLY IMMEDIATE
;

-- Reference: User_Role_User (table: users_roles)
ALTER TABLE users_roles ADD CONSTRAINT User_Role_User
    FOREIGN KEY (user_id)
    REFERENCES users (user_id)
    ON DELETE  RESTRICT  
    NOT DEFERRABLE 
    INITIALLY IMMEDIATE
;

CREATE INDEX idx_recipes_author
ON recipes(author_id);

CREATE INDEX idx_recipes_category
ON recipes(category_id);

CREATE INDEX idx_recipes_cuisine
ON recipes(cuisine_id);

CREATE INDEX idx_recipe_ingredients_ingredient
ON recipe_ingredients(ingredient_id);

CREATE INDEX idx_user_favorites_recipe
ON user_favorites(recipe_id);