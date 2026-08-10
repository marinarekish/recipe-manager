using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Application.Contracts.Favorites;
using RecipeManager.Application.Interfaces;
using RecipeManager.Domain.Entities;
using RecipeManager.Infrastructure.Persistence;

namespace RecipeManager.Application.Services;

public class FavoriteService(
    IMapper mapper, 
    ApplicationDbContext context) : IFavoriteService
{
    public async Task<List<FavoriteRecipeResponse>> GetUserFavoritesAsync(
        int userId, 
        CancellationToken ct = default)
    {
        return await context.UserFavorites
            .AsNoTracking()
            .Where(uf => uf.UserId == userId)
            .ProjectTo<FavoriteRecipeResponse>(mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    public async Task<FavoriteRecipeResponse?> AddFavoriteAsync(
        int userId, 
        int recipeId, 
        CancellationToken ct = default)
    {
        var alreadyExists = await IsFavoriteAsync(userId, recipeId, ct);

        if (alreadyExists)
            return null;
        
        var recipeExists = await context.Recipes
            .AsNoTracking()
            .AnyAsync(r => r.RecipeId == recipeId, ct);
        
        if (!recipeExists)
            return null;

        var favoriteToAdd = new UserFavorite
        {
            UserId = userId,
            RecipeId = recipeId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        
        context.UserFavorites.Add(favoriteToAdd);
        await context.SaveChangesAsync(ct);
        
        return await context.UserFavorites
            .AsNoTracking()
            .Where(f => f.UserId == userId && f.RecipeId == recipeId)
            .ProjectTo<FavoriteRecipeResponse>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<bool> RemoveFavoriteAsync(
        int userId, 
        int recipeId, 
        CancellationToken ct = default)
    {
        var favToRemove = await context.UserFavorites
            .FirstOrDefaultAsync(uf => uf.UserId == userId && uf.RecipeId == recipeId, ct);
        
        if (favToRemove == null)
            return false;
        
        context.UserFavorites.Remove(favToRemove);
        await context.SaveChangesAsync(ct);
        
        return true;
    }

    public async Task<bool> IsFavoriteAsync(
        int userId, 
        int recipeId, 
        CancellationToken ct = default)
    {
        return await context.UserFavorites
            .AsNoTracking()
            .AnyAsync(uf => uf.UserId == userId && uf.RecipeId == recipeId, ct);
    }
}