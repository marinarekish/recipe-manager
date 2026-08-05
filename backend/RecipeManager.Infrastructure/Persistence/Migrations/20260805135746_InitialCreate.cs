using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RecipeManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    category_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.category_id);
                });

            migrationBuilder.CreateTable(
                name: "cuisines",
                columns: table => new
                {
                    cuisine_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cuisines", x => x.cuisine_id);
                });

            migrationBuilder.CreateTable(
                name: "ingredients",
                columns: table => new
                {
                    ingredient_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ingredients", x => x.ingredient_id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    first_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    last_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "recipes",
                columns: table => new
                {
                    recipe_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    author_id = table.Column<int>(type: "integer", nullable: false),
                    cuisine_id = table.Column<int>(type: "integer", nullable: false),
                    category_id = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    prep_time_minutes = table.Column<int>(type: "integer", nullable: false),
                    cook_time_minutes = table.Column<int>(type: "integer", nullable: false),
                    servings = table.Column<int>(type: "integer", nullable: false),
                    instructions = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipes", x => x.recipe_id);
                    table.CheckConstraint("CK_recipes_cook_time", "cook_time_minutes > 0");
                    table.CheckConstraint("CK_recipes_prep_time", "prep_time_minutes > 0");
                    table.CheckConstraint("CK_recipes_servings", "servings > 0");
                    table.ForeignKey(
                        name: "FK_recipes_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "category_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_recipes_cuisines_cuisine_id",
                        column: x => x.cuisine_id,
                        principalTable: "cuisines",
                        principalColumn: "cuisine_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_recipes_users_author_id",
                        column: x => x.author_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "users_roles",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users_roles", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "FK_users_roles_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_users_roles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recipe_ingredients",
                columns: table => new
                {
                    recipe_id = table.Column<int>(type: "integer", nullable: false),
                    ingredient_id = table.Column<int>(type: "integer", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    unit = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipe_ingredients", x => new { x.recipe_id, x.ingredient_id });
                    table.CheckConstraint("check_amount", "amount > 0");
                    table.ForeignKey(
                        name: "FK_recipe_ingredients_ingredients_ingredient_id",
                        column: x => x.ingredient_id,
                        principalTable: "ingredients",
                        principalColumn: "ingredient_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_recipe_ingredients_recipes_recipe_id",
                        column: x => x.recipe_id,
                        principalTable: "recipes",
                        principalColumn: "recipe_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_favorites",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    recipe_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_favorites", x => new { x.recipe_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_user_favorites_recipes_recipe_id",
                        column: x => x.recipe_id,
                        principalTable: "recipes",
                        principalColumn: "recipe_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_favorites_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "categories",
                columns: new[] { "category_id", "name" },
                values: new object[,]
                {
                    { 1, "Breakfast" },
                    { 2, "Lunch" },
                    { 3, "Dinner" },
                    { 4, "Dessert" },
                    { 5, "Salad" },
                    { 6, "Drinks" },
                    { 7, "Soup" },
                    { 8, "Main Course" }
                });

            migrationBuilder.InsertData(
                table: "cuisines",
                columns: new[] { "cuisine_id", "name" },
                values: new object[,]
                {
                    { 1, "Italian" },
                    { 2, "English" },
                    { 3, "French" },
                    { 4, "Mediterranean" },
                    { 5, "Spanish" },
                    { 6, "Slavic" }
                });

            migrationBuilder.InsertData(
                table: "ingredients",
                columns: new[] { "ingredient_id", "name" },
                values: new object[,]
                {
                    { 1, "Milk" },
                    { 2, "Sugar" },
                    { 3, "Potato" },
                    { 4, "Rice" },
                    { 5, "Sour cream" },
                    { 6, "Salt" },
                    { 7, "Egg" },
                    { 8, "Pork meat" },
                    { 9, "Cheese" },
                    { 10, "Chicken breast" },
                    { 11, "Tomato" },
                    { 12, "Carrot" },
                    { 13, "Olive Oil" },
                    { 14, "Pepperoni" }
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "role_id", "name" },
                values: new object[,]
                {
                    { 1, "Administrator" },
                    { 2, "User" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_categories_name",
                table: "categories",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cuisines_name",
                table: "cuisines",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ingredients_name",
                table: "ingredients",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_recipe_ingredients_ingredient_id",
                table: "recipe_ingredients",
                column: "ingredient_id");

            migrationBuilder.CreateIndex(
                name: "IX_recipes_author_id",
                table: "recipes",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_recipes_category_id",
                table: "recipes",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_recipes_cuisine_id",
                table: "recipes",
                column: "cuisine_id");

            migrationBuilder.CreateIndex(
                name: "IX_roles_name",
                table: "roles",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_favorites_user_id",
                table: "user_favorites",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_roles_role_id",
                table: "users_roles",
                column: "role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "recipe_ingredients");

            migrationBuilder.DropTable(
                name: "user_favorites");

            migrationBuilder.DropTable(
                name: "users_roles");

            migrationBuilder.DropTable(
                name: "ingredients");

            migrationBuilder.DropTable(
                name: "recipes");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "cuisines");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
