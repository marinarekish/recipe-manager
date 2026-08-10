namespace RecipeManager.Application.Contracts.Recipes;

public record RecipeIngredientResponse(
    int IngredientId,
    string Name,
    decimal Amount,
    string Unit
);