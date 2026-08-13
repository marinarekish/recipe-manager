using System.ComponentModel.DataAnnotations;

namespace RecipeManager.Application.Contracts.Ingredients;

public record CreateIngredientRequest(
    [property: Required, StringLength(50), RegularExpression(@"\S")] string Name
);
