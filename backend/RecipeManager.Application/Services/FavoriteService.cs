using RecipeManager.Application.Interfaces;
using RecipeManager.Domain.Entities;
using RecipeManager.Infrastructure.Persistence;

namespace RecipeManager.Application.Services;

public class FavoriteService(ApplicationDbContext context) : IFavoriteService
{
    private readonly ApplicationDbContext _context = context;
    public Task<List<Recipe>> GetUserFavoritesAsync(int userId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task AddFavoriteAsync(int userId, int recipeId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task RemoveFavoriteAsync(int userId, int recipeId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsFavoriteAsync(int userId, int recipeId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}