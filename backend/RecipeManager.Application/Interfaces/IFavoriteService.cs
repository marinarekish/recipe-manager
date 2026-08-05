using RecipeManager.Domain.Entities;

namespace RecipeManager.Application.Interfaces;

public interface IFavoriteService
{
    Task<List<Recipe>> GetUserFavoritesAsync(int userId, CancellationToken ct = default);
    
    Task AddFavoriteAsync(int userId, int recipeId, CancellationToken ct = default);
    Task RemoveFavoriteAsync(int userId, int recipeId, CancellationToken ct = default);
    
    Task<bool> IsFavoriteAsync(int userId, int recipeId, CancellationToken ct = default);
}