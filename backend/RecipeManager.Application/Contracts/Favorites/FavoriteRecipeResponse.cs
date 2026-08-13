namespace RecipeManager.Application.Contracts.Favorites;

public class FavoriteRecipeResponse()
{
    public int RecipeId { get; init; }
    public string Title { get; init; } = null!;
    public DateTime AddedAt { get; init; }
}