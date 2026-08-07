namespace RecipeManager.Application.Contracts.Recipes;

public record RecipeResponse(
    int RecipeId,
    string Title,
    int PrepTimeMinutes,
    int CookTimeMinutes,
    int Servings,
    string? Instructions,
    int CuisineId,
    int CategoryId,
    List<RecipeIngredientResponse> Ingredients
    );