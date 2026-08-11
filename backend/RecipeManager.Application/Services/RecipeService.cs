using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Application.Contracts.Recipes;
using RecipeManager.Application.Interfaces;
using RecipeManager.Domain.Entities;
using RecipeManager.Infrastructure.Persistence;

namespace RecipeManager.Application.Services;

public class RecipeService(
    IMapper mapper,
    ApplicationDbContext context) : IRecipeService
{
    public async Task<List<RecipeResponse>> GetAllRecipesByAdminAsync(
        CancellationToken ct = default)
    {
        return await context.Recipes
            .AsNoTracking()
            .ProjectTo<RecipeResponse>(mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    public async Task<RecipeResponse?> GetRecipeByIdAsync(
        int recipeId,
        CancellationToken ct = default)
    {
        return await context.Recipes
            .AsNoTracking()
            .Where(r => r.RecipeId == recipeId)
            .ProjectTo<RecipeResponse>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<List<RecipeResponse>?> GetAllRecipesByUserIdAsync(
        int authorId,
        CancellationToken ct = default)
    {
        var authorExists = await context.Users
            .AsNoTracking()
            .AnyAsync(u => u.UserId == authorId, ct);

        if (!authorExists)
            return null;

        return await context.Recipes
            .AsNoTracking()
            .Where(r => r.AuthorId == authorId)
            .ProjectTo<RecipeResponse>(mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    public async Task<RecipeResponse?> CreateRecipeAsync(
        int authorId,
        CreateRecipeRequest recipe,
        CancellationToken ct = default)
    {
        var authorExists = await context.Users
            .AsNoTracking()
            .AnyAsync(u => u.UserId == authorId, ct);

        if (!authorExists)
            return null;

        var recipeToAdd = mapper.Map<Recipe>(recipe);

        recipeToAdd.AuthorId = authorId;
        recipeToAdd.CreatedAt = DateTime.UtcNow;
        recipeToAdd.UpdatedAt = DateTime.UtcNow;

        foreach (var item in recipe.Ingredients)
        {
            recipeToAdd.RecipeIngredients.Add(new RecipeIngredient
            {
                IngredientId = item.IngredientId,
                Amount = item.Amount,
                Unit = item.Unit
            });
        }

        context.Recipes.Add(recipeToAdd);
        await context.SaveChangesAsync(ct);

        return await context.Recipes
            .AsNoTracking()
            .Where(r => r.RecipeId == recipeToAdd.RecipeId)
            .ProjectTo<RecipeResponse>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }

    /// <summary>
    /// Updates recipe by id only.
    /// Authorization (owner vs admin) must be enforced in the controller.
    /// </summary>
    public async Task<RecipeResponse?> UpdateRecipeAsync(
        int recipeId,
        UpdateRecipeRequest recipe,
        CancellationToken ct = default)
    {
        var recipeToUpdate = await context.Recipes
            .Include(r => r.RecipeIngredients)
            .FirstOrDefaultAsync(r => r.RecipeId == recipeId, ct);

        if (recipeToUpdate is null)
            return null;

        mapper.Map(recipe, recipeToUpdate);
        recipeToUpdate.UpdatedAt = DateTime.UtcNow;

        if (recipe.Ingredients is not null)
        {
            recipeToUpdate.RecipeIngredients.Clear();

            foreach (var item in recipe.Ingredients)
            {
                recipeToUpdate.RecipeIngredients.Add(new RecipeIngredient
                {
                    RecipeId = recipeId,
                    IngredientId = item.IngredientId,
                    Amount = item.Amount,
                    Unit = item.Unit
                });
            }
        }

        await context.SaveChangesAsync(ct);

        return await context.Recipes
            .AsNoTracking()
            .Where(r => r.RecipeId == recipeId)
            .ProjectTo<RecipeResponse>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }

    /// <summary>
    /// Deletes recipe by id only.
    /// Authorization must be enforced in the controller.
    /// </summary>
    public async Task<bool> DeleteRecipeAsync(
        int recipeId,
        CancellationToken ct = default)
    {
        var recipeToDelete = await context.Recipes
            .FirstOrDefaultAsync(r => r.RecipeId == recipeId, ct);

        if (recipeToDelete is null)
            return false;

        context.Recipes.Remove(recipeToDelete);
        await context.SaveChangesAsync(ct);

        return true;
    }
}