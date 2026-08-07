-- Created by Redgate Data Modeler (https://datamodeler.redgate-platform.com)
-- Last modification date: 2026-08-06 11:26:22.705

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

-- Table: login_tokens
CREATE TABLE login_tokens (
    login_token_id int  NOT NULL,
    user_id int  NOT NULL,
    code_hash varchar(255)  NOT NULL,
    created_at timestamp  NOT NULL DEFAULT CURRENT_TIMESTAMP,
    expires_at timestamp  NOT NULL,
    used_at timestamp  NOT NULL,
    CONSTRAINT login_tokens_pk PRIMARY KEY (login_token_id)
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

-- Reference: ingredients_recingredients (table: recipe_ingredients)
ALTER TABLE recipe_ingredients ADD CONSTRAINT ingredients_recingredients
    FOREIGN KEY (ingredient_id)
    REFERENCES ingredients (ingredient_id)
    ON DELETE  RESTRICT  
    NOT DEFERRABLE 
    INITIALLY IMMEDIATE
;

-- Reference: login_users (table: login_tokens)
ALTER TABLE login_tokens ADD CONSTRAINT login_users
    FOREIGN KEY (user_id)
    REFERENCES users (user_id)  
    NOT DEFERRABLE 
    INITIALLY IMMEDIATE
;

-- Reference: recipe_category (table: recipes)
ALTER TABLE recipes ADD CONSTRAINT recipe_category
    FOREIGN KEY (category_id)
    REFERENCES categories (category_id)
    ON DELETE  RESTRICT  
    NOT DEFERRABLE 
    INITIALLY IMMEDIATE
;

-- Reference: recipe_cuisine (table: recipes)
ALTER TABLE recipes ADD CONSTRAINT recipe_cuisine
    FOREIGN KEY (cuisine_id)
    REFERENCES cuisines (cuisine_id)
    ON DELETE  RESTRICT  
    NOT DEFERRABLE 
    INITIALLY IMMEDIATE
;

-- Reference: recipe_recingr (table: recipe_ingredients)
ALTER TABLE recipe_ingredients ADD CONSTRAINT recipe_recingr
    FOREIGN KEY (recipe_id)
    REFERENCES recipes (recipe_id)
    ON DELETE  CASCADE  
    NOT DEFERRABLE 
    INITIALLY IMMEDIATE
;

-- Reference: recipe_user (table: recipes)
ALTER TABLE recipes ADD CONSTRAINT recipe_user
    FOREIGN KEY (author_id)
    REFERENCES users (user_id)
    ON DELETE  RESTRICT  
    NOT DEFERRABLE 
    INITIALLY IMMEDIATE
;

-- Reference: recipe_userfav (table: user_favorites)
ALTER TABLE user_favorites ADD CONSTRAINT recipe_userfav
    FOREIGN KEY (recipe_id)
    REFERENCES recipes (recipe_id)
    ON DELETE  CASCADE  
    NOT DEFERRABLE 
    INITIALLY IMMEDIATE
;

-- Reference: user_userfav (table: user_favorites)
ALTER TABLE user_favorites ADD CONSTRAINT user_userfav
    FOREIGN KEY (user_id)
    REFERENCES users (user_id)
    ON DELETE  CASCADE  
    NOT DEFERRABLE 
    INITIALLY IMMEDIATE
;

-- End of file.

