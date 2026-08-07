namespace RecipeManager.Application.Contracts.Recipes;

public record CreateRecipeRequest(
    string Title,
    int CuisineId,
    int CategoryId,
    int PrepTimeMinutes,
    int CookTimeMinutes,
    int Servings,
    string? Instructions,
    
    List<RecipeIngredientRequest> Ingredients
    );