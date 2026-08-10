using RecipeManager.Application.Contracts.Ingredients;

namespace RecipeManager.Application.Contracts.Recipes;

public record RecipeIngredientRequest(
    int IngredientId,
    decimal Amount,
    string Unit
);