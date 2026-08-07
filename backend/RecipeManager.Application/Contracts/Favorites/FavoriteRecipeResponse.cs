namespace RecipeManager.Application.Contracts.Favorites;

public record FavoriteRecipeResponse(
    int RecipeId,
    string Title,
    DateTime AddedAt 
);