using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class WidenIngredientAmount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "amount",
                table: "recipe_ingredients",
                type: "numeric(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,2)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "amount",
                table: "recipe_ingredients",
                type: "numeric(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,2)");
        }
    }
}
