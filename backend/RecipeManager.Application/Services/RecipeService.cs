using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Application.Common.Results;
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
    public async Task<List<RecipeResponse>> GetAllRecipesAsync(
        CancellationToken ct = default)
    {
        return await context.Recipes
            .AsNoTracking()
            .ProjectTo<RecipeResponse>(mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    public async Task<Result<RecipeResponse>> GetRecipeByIdAsync(
        int recipeId,
        CancellationToken ct = default)
    {
        var recipe = await context.Recipes
            .AsNoTracking()
            .Where(r => r.RecipeId == recipeId)
            .ProjectTo<RecipeResponse>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);

        return recipe is null
            ? Result<RecipeResponse>.NotFound()
            : Result<RecipeResponse>.Ok(recipe);
    }

    public async Task<Result<List<RecipeResponse>>> GetAllRecipesByUserIdAsync(
        int authorId,
        CancellationToken ct = default)
    {
        var authorExists = await context.Users
            .AsNoTracking()
            .AnyAsync(u => u.UserId == authorId, ct);

        if (!authorExists)
            return Result<List<RecipeResponse>>.NotFound();

        var recipes = await context.Recipes
            .AsNoTracking()
            .Where(r => r.AuthorId == authorId)
            .ProjectTo<RecipeResponse>(mapper.ConfigurationProvider)
            .ToListAsync(ct);

        return Result<List<RecipeResponse>>.Ok(recipes);
    }

    public async Task<Result<RecipeResponse>> CreateRecipeAsync(
        int authorId,
        CreateRecipeRequest recipe,
        CancellationToken ct = default)
    {
        var authorExists = await context.Users
            .AsNoTracking()
            .AnyAsync(u => u.UserId == authorId, ct);

        if (!authorExists)
            return Result<RecipeResponse>.NotFound();

        try
        {
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
                    return Result<RecipeResponse>.ValidationError(
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

            var createdRecipe = await context.Recipes
                .AsNoTracking()
                .Where(r => r.RecipeId == recipeToAdd.RecipeId)
                .ProjectTo<RecipeResponse>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(ct);

            return Result<RecipeResponse>.Ok(createdRecipe!);
        }
        catch (ArgumentException ex)
        {
            return Result<RecipeResponse>.ValidationError(ex.Message);
        }
    }

    /// <summary>
    /// Updates a recipe by id. Ownership is enforced here:
    /// a non-admin can only update their own recipes.
    /// </summary>
    public async Task<Result<RecipeResponse>> UpdateRecipeAsync(
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
            return Result<RecipeResponse>.NotFound();

        if (!isAdmin && recipeToUpdate.AuthorId != currentUserId)
            return Result<RecipeResponse>.Forbidden();

        try
        {
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
            if (recipe.ImageUrl is not null)
                recipeToUpdate.ImageUrl = recipe.ImageUrl;

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
                        return Result<RecipeResponse>.ValidationError(
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

            return Result<RecipeResponse>.Ok(updatedRecipe!);
        }
        catch (ArgumentException ex)
        {
            return Result<RecipeResponse>.ValidationError(ex.Message);
        }
    }

    /// <summary>
    /// Deletes a recipe by id. Ownership is enforced here:
    /// a non-admin can only delete their own recipes.
    /// </summary>
    public async Task<Result> DeleteRecipeAsync(
        int recipeId,
        int currentUserId,
        bool isAdmin,
        CancellationToken ct = default)
    {
        var recipeToDelete = await context.Recipes
            .FirstOrDefaultAsync(r => r.RecipeId == recipeId, ct);

        if (recipeToDelete is null)
            return Result.NotFound();

        if (!isAdmin && recipeToDelete.AuthorId != currentUserId)
            return Result.Forbidden();

        context.Recipes.Remove(recipeToDelete);
        await context.SaveChangesAsync(ct);

        return Result.Ok();
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
            var result = await cuisineService.GetOrCreateAsync(cuisineName, ct);
            return result.Value!.CuisineId;
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
            var result = await categoryService.GetOrCreateAsync(categoryName, ct);
            return result.Value!.CategoryId;
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
        
        var result = await ingredientService.GetOrCreateAsync(ingredientName, ct);
        return result.Value!.IngredientId;
    }
}
