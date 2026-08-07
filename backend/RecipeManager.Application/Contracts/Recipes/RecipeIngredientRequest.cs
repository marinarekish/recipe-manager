using RecipeManager.Application.Contracts.Ingredients;

namespace RecipeManager.Application.Contracts.Recipes;

public record RecipeIngredientRequest(
    IngredientResponse Ingredient,
    decimal Amount,
    string Unit
);