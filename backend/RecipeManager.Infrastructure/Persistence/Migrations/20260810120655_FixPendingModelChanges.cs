using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixPendingModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_user_favorites",
                table: "user_favorites");

            migrationBuilder.DropIndex(
                name: "IX_user_favorites_user_id",
                table: "user_favorites");

            migrationBuilder.DropCheckConstraint(
                name: "check_amount",
                table: "recipe_ingredients");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_favorites",
                table: "user_favorites",
                columns: new[] { "user_id", "recipe_id" });

            migrationBuilder.CreateIndex(
                name: "IX_user_favorites_recipe_id",
                table: "user_favorites",
                column: "recipe_id");

            migrationBuilder.AddCheckConstraint(
                name: "CK_recipe_ingredients_amount",
                table: "recipe_ingredients",
                sql: "amount > 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_user_favorites",
                table: "user_favorites");

            migrationBuilder.DropIndex(
                name: "IX_user_favorites_recipe_id",
                table: "user_favorites");

            migrationBuilder.DropCheckConstraint(
                name: "CK_recipe_ingredients_amount",
                table: "recipe_ingredients");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_favorites",
                table: "user_favorites",
                columns: new[] { "recipe_id", "user_id" });

            migrationBuilder.CreateIndex(
                name: "IX_user_favorites_user_id",
                table: "user_favorites",
                column: "user_id");

            migrationBuilder.AddCheckConstraint(
                name: "check_amount",
                table: "recipe_ingredients",
                sql: "amount > 0");
        }
    }
}
