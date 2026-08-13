using System.ComponentModel.DataAnnotations;

namespace RecipeManager.Application.Contracts.Recipes;

public record RecipeIngredientRequest(
    [property: Range(1, int.MaxValue)] int? IngredientId,
    [property: StringLength(50), RegularExpression(@"\S")] string? Name,
    [property: Range(0.01, 99999999.99)] decimal Amount,
    [property: Required, StringLength(10)] string Unit
);
