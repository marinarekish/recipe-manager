namespace RecipeManager.Application.Contracts.Recipes;

public class RecipeResponse
{
    public int RecipeId { get; init; }
    public string Title { get; init; } = null!;
    public int PrepTimeMinutes { get; init; }
    public int CookTimeMinutes { get; init; }
    public int Servings { get; init; }
    public string? Instructions { get; init; }
    public int CuisineId { get; init; }
    public string CuisineName { get; init; } = null!;
    public int CategoryId { get; init; }
    public string CategoryName { get; init; } = null!;
    public int AuthorId { get; init; }
    public string AuthorName { get; init; } = null!;
    public List<RecipeIngredientResponse> Ingredients { get; init; } = new();
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}