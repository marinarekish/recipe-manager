using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Application.Common.Results;
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

    public async Task<Result<FavoriteRecipeResponse>> AddFavoriteAsync(
        int userId, 
        int recipeId, 
        CancellationToken ct = default)
    {
        var alreadyExists = await IsFavoriteAsync(userId, recipeId, ct);

        if (alreadyExists)
            return Result<FavoriteRecipeResponse>.Conflict(
                "Recipe is already in favorites.");
        
        var recipeExists = await context.Recipes
            .AsNoTracking()
            .AnyAsync(r => r.RecipeId == recipeId, ct);
        
        if (!recipeExists)
            return Result<FavoriteRecipeResponse>.NotFound();

        var favoriteToAdd = new UserFavorite
        {
            UserId = userId,
            RecipeId = recipeId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        
        context.UserFavorites.Add(favoriteToAdd);
        await context.SaveChangesAsync(ct);
        
        var created = await context.UserFavorites
            .AsNoTracking()
            .Where(f => f.UserId == userId && f.RecipeId == recipeId)
            .ProjectTo<FavoriteRecipeResponse>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);

        return Result<FavoriteRecipeResponse>.Ok(created!);
    }

    public async Task<Result> RemoveFavoriteAsync(
        int userId, 
        int recipeId, 
        CancellationToken ct = default)
    {
        var favToRemove = await context.UserFavorites
            .FirstOrDefaultAsync(uf => uf.UserId == userId && uf.RecipeId == recipeId, ct);
        
        if (favToRemove == null)
            return Result.NotFound();
        
        context.UserFavorites.Remove(favToRemove);
        await context.SaveChangesAsync(ct);
        
        return Result.Ok();
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