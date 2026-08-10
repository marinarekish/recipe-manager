using RecipeManager.Application.Contracts.Favorites;

namespace RecipeManager.Application.Interfaces;

public interface IFavoriteService
{
    Task<List<FavoriteRecipeResponse>> GetUserFavoritesAsync(
        int userId, 
        CancellationToken ct = default);
    
    Task<FavoriteRecipeResponse?> AddFavoriteAsync(
        int userId, 
        int recipeId, 
        CancellationToken ct = default);
    
    Task<bool> RemoveFavoriteAsync(
        int userId, 
        int recipeId, 
        CancellationToken ct = default);
    
    Task<bool> IsFavoriteAsync(
        int userId, 
        int recipeId, 
        CancellationToken ct = default);
}