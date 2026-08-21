using System.ComponentModel.DataAnnotations;

namespace RecipeManager.Application.Contracts.Recipes;

public record CreateRecipeRequest(
    [Required, StringLength(100), RegularExpression(@"^.*\S.*$")] string Title,
    [Range(1, int.MaxValue)] int? CuisineId,
    [StringLength(50), RegularExpression(@"^.*\S.*$")] string? CuisineName,
    [Range(1, int.MaxValue)] int? CategoryId,
    [StringLength(50), RegularExpression(@"^.*\S.*$")] string? CategoryName,
    [Range(1, int.MaxValue)] int PrepTimeMinutes,
    [Range(1, int.MaxValue)] int CookTimeMinutes,
    [Range(1, int.MaxValue)] int Servings,
    string? Instructions,
    [StringLength(2048), RegularExpression(@"^.*\S.*$")] string? ImageUrl,
    [Required, MinLength(1)] List<RecipeIngredientRequest> Ingredients
);