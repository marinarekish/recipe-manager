namespace RecipeManager.Application.Contracts.Recipes;

public record RecipeResponse(
    int RecipeId,
    string Title,
    int PrepTimeMinutes,
    int CookTimeMinutes,
    int Servings,
    string? Instructions,
    int CuisineId,
    string CuisineName,
    int CategoryId,
    string CategoryName,
    int AuthorId,
    string AuthorName,
    List<RecipeIngredientResponse> Ingredients,
    DateTime CreatedAt,
    DateTime UpdatedAt
);