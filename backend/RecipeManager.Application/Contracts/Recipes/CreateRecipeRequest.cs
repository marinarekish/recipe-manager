using System.ComponentModel.DataAnnotations;

namespace RecipeManager.Application.Contracts.Recipes;

public record CreateRecipeRequest(
    [property: Required, StringLength(100), RegularExpression(@"\S")] string Title,
    [property: Range(1, int.MaxValue)] int? CuisineId,
    [property: StringLength(50), RegularExpression(@"\S")] string? CuisineName,
    [property: Range(1, int.MaxValue)] int? CategoryId,
    [property: StringLength(50), RegularExpression(@"\S")] string? CategoryName,
    [property: Range(1, int.MaxValue)] int PrepTimeMinutes,
    [property: Range(1, int.MaxValue)] int CookTimeMinutes,
    [property: Range(1, int.MaxValue)] int Servings,
    string? Instructions,

    [property: Required, MinLength(1)] List<RecipeIngredientRequest> Ingredients
);
