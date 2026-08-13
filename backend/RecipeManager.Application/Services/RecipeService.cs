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
    ApplicationDbContext context,
    ICuisineService cuisineService,
    ICategoryService categoryService,
    IIngredientService ingredientService) : IRecipeService
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
        recipeToAdd.CuisineId = await ResolveCuisineIdAsync(recipe.CuisineId, recipe.CuisineName, ct);
        recipeToAdd.CategoryId = await ResolveCategoryIdAsync(recipe.CategoryId, recipe.CategoryName, ct);
        recipeToAdd.CreatedAt = DateTime.UtcNow;
        recipeToAdd.UpdatedAt = DateTime.UtcNow;

        var ingredientIds = new HashSet<int>();

        foreach (var item in recipe.Ingredients)
        {
            var ingredientId = await ResolveIngredientIdAsync(item.IngredientId, item.Name, ct);

            if (!ingredientIds.Add(ingredientId))
                throw new ArgumentException(
                    $"Duplicate ingredient in request (id={ingredientId}).");

            recipeToAdd.RecipeIngredients.Add(new RecipeIngredient
            {
                IngredientId = ingredientId,
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
    /// Updates a recipe by id. Ownership is enforced here:
    /// a non-admin can only update their own recipes.
    /// </summary>
    public async Task<RecipeUpdateResult> UpdateRecipeAsync(
        int recipeId,
        int currentUserId,
        bool isAdmin,
        UpdateRecipeRequest recipe,
        CancellationToken ct = default)
    {
        var recipeToUpdate = await context.Recipes
            .Include(r => r.RecipeIngredients)
            .FirstOrDefaultAsync(r => r.RecipeId == recipeId, ct);

        if (recipeToUpdate is null)
            return new RecipeUpdateResult(RecipeOperationStatus.NotFound, null);

        if (!isAdmin && recipeToUpdate.AuthorId != currentUserId)
            return new RecipeUpdateResult(RecipeOperationStatus.Forbidden, null);

        // Partial update: only non-null scalars (safer than relying only on AutoMapper)
        if (recipe.Title is not null)
            recipeToUpdate.Title = recipe.Title;
        if (recipe.PrepTimeMinutes is not null)
            recipeToUpdate.PrepTimeMinutes = recipe.PrepTimeMinutes.Value;
        if (recipe.CookTimeMinutes is not null)
            recipeToUpdate.CookTimeMinutes = recipe.CookTimeMinutes.Value;
        if (recipe.Servings is not null)
            recipeToUpdate.Servings = recipe.Servings.Value;
        if (recipe.Instructions is not null)
            recipeToUpdate.Instructions = recipe.Instructions;

        if (recipe.CuisineId is not null || !string.IsNullOrWhiteSpace(recipe.CuisineName))
            recipeToUpdate.CuisineId = await ResolveCuisineIdAsync(recipe.CuisineId, recipe.CuisineName, ct);

        if (recipe.CategoryId is not null || !string.IsNullOrWhiteSpace(recipe.CategoryName))
            recipeToUpdate.CategoryId = await ResolveCategoryIdAsync(recipe.CategoryId, recipe.CategoryName, ct);
        
        recipeToUpdate.UpdatedAt = DateTime.UtcNow;

        if (recipe.Ingredients is not null)
        {
            recipeToUpdate.RecipeIngredients.Clear();

            var ingredientIds = new HashSet<int>();

            foreach (var item in recipe.Ingredients)
            {
                var ingredientId = await ResolveIngredientIdAsync(item.IngredientId, item.Name, ct);

                if (!ingredientIds.Add(ingredientId))
                    throw new ArgumentException(
                        $"Duplicate ingredient in request (id={ingredientId}).");

                recipeToUpdate.RecipeIngredients.Add(new RecipeIngredient
                {
                    RecipeId = recipeId,
                    IngredientId = ingredientId,
                    Amount = item.Amount,
                    Unit = item.Unit
                });
            }
        }

        await context.SaveChangesAsync(ct);

        var updatedRecipe = await context.Recipes
            .AsNoTracking()
            .Where(r => r.RecipeId == recipeId)
            .ProjectTo<RecipeResponse>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);

        return new RecipeUpdateResult(RecipeOperationStatus.Ok, updatedRecipe);
    }

    /// <summary>
    /// Deletes a recipe by id. Ownership is enforced here:
    /// a non-admin can only delete their own recipes.
    /// </summary>
    public async Task<RecipeOperationStatus> DeleteRecipeAsync(
        int recipeId,
        int currentUserId,
        bool isAdmin,
        CancellationToken ct = default)
    {
        var recipeToDelete = await context.Recipes
            .FirstOrDefaultAsync(r => r.RecipeId == recipeId, ct);

        if (recipeToDelete is null)
            return RecipeOperationStatus.NotFound;

        if (!isAdmin && recipeToDelete.AuthorId != currentUserId)
            return RecipeOperationStatus.Forbidden;

        context.Recipes.Remove(recipeToDelete);
        await context.SaveChangesAsync(ct);

        return RecipeOperationStatus.Ok;
    }

    /// <summary>
    /// Resolves a cuisine id from the request: a valid id wins,
    /// otherwise the name is looked up (or created).
    /// </summary>
    private async Task<int> ResolveCuisineIdAsync(
        int? cuisineId,
        string? cuisineName,
        CancellationToken ct)
    {
        if (cuisineId.HasValue)
        {
            var exists = await context.Cuisines
                .AsNoTracking()
                .AnyAsync(c => c.CuisineId == cuisineId.Value, ct);

            if (!exists)
                throw new ArgumentException($"Cuisine id {cuisineId.Value} was not found.");

            return cuisineId.Value;
        }

        if (!string.IsNullOrWhiteSpace(cuisineName))
        {
            var cuisine = await cuisineService.GetOrCreateAsync(cuisineName, ct);
            return cuisine!.CuisineId;
        }

        throw new ArgumentException("Cuisine must be provided as an id or a name.");
    }

    /// <summary>
    /// Resolves a category id from the request: a valid id wins,
    /// otherwise the name is looked up (or created).
    /// </summary>
    private async Task<int> ResolveCategoryIdAsync(
        int? categoryId,
        string? categoryName,
        CancellationToken ct)
    {
        if (categoryId.HasValue)
        {
            var exists = await context.Categories
                .AsNoTracking()
                .AnyAsync(c => c.CategoryId == categoryId.Value, ct);

            if (!exists)
                throw new ArgumentException($"Category id {categoryId.Value} was not found.");

            return categoryId.Value;
        }

        if (!string.IsNullOrWhiteSpace(categoryName))
        {
            var category = await categoryService.GetOrCreateAsync(categoryName, ct);
            return category!.CategoryId;
        }

        throw new ArgumentException("Category must be provided as an id or a name.");
    }

    /// <summary>
    /// Resolves an ingredient id from the request: a valid id wins,
    /// otherwise the name is looked up (or created).
    /// </summary>
    private async Task<int> ResolveIngredientIdAsync(
        int? ingredientId,
        string? ingredientName,
        CancellationToken ct)
    {
        if (ingredientId.HasValue)
        {
            var exists = await context.Ingredients
                .AsNoTracking()
                .AnyAsync(i => i.IngredientId == ingredientId.Value, ct);

            if (!exists)
                throw new ArgumentException($"Ingredient id {ingredientId.Value} was not found.");

            return ingredientId.Value;
        }

        if (string.IsNullOrWhiteSpace(ingredientName))
            throw new ArgumentException("Ingredient must be provided as an id or a name.");
        
        var ingredient = await ingredientService.GetOrCreateAsync(ingredientName, ct);
        return ingredient!.IngredientId;
    }
}
