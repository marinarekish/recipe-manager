using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Application.Common.Results;
using RecipeManager.Application.Contracts.Ingredients;
using RecipeManager.Application.Interfaces;
using RecipeManager.Domain.Entities;
using RecipeManager.Infrastructure.Persistence;

namespace RecipeManager.Application.Services;

public class IngredientService(
    IMapper mapper,
    ApplicationDbContext context) : IIngredientService
{
    public async Task<List<IngredientResponse>> GetAllIngredientsAsync(CancellationToken ct = default)
    {
        return await context.Ingredients
            .AsNoTracking()
            .ProjectTo<IngredientResponse>(mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    public async Task<Result<IngredientResponse>> GetIngredientByIdAsync(int id, CancellationToken ct = default)
    {
        var ingredient = await context.Ingredients
            .AsNoTracking()
            .Where(i => i.IngredientId == id)
            .ProjectTo<IngredientResponse>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);

        return ingredient is not null
            ? Result<IngredientResponse>.Ok(ingredient)
            : Result<IngredientResponse>.NotFound();
    }

    public async Task<Result<IngredientResponse>> GetOrCreateAsync(string name, CancellationToken ct = default)
    {
        var trimmedName = name.Trim();

        if (string.IsNullOrWhiteSpace(trimmedName))
            return Result<IngredientResponse>.ValidationError("Ingredient name cannot be empty.");

        var storeName = trimmedName.ToLowerInvariant();

        var ingredient = await context.Ingredients
            .FirstOrDefaultAsync(
                i => i.Name.ToLower() == storeName.ToLower(),
                ct);

        if (ingredient is not null)
            return Result<IngredientResponse>.Ok(mapper.Map<IngredientResponse>(ingredient));

        ingredient = new Ingredient { Name = trimmedName };
        context.Ingredients.Add(ingredient);

        try
        {
            await context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            ingredient = await context.Ingredients
                .FirstAsync(i => i.Name.ToLower() == trimmedName.ToLower(), ct);
        }

        return Result<IngredientResponse>.Ok(mapper.Map<IngredientResponse>(ingredient));
    }

    public async Task<Result> DeleteIngredientAsync(int id, CancellationToken ct = default)
    {
        var ingredientToDelete = await context.Ingredients
            .FirstOrDefaultAsync(i => i.IngredientId == id, ct);

        if (ingredientToDelete is null)
            return Result.NotFound();

        var inUse = await context.RecipeIngredients
            .AsNoTracking()
            .AnyAsync(ri => ri.IngredientId == id, ct);

        if (inUse)
            return Result.Conflict("Ingredient is used by one or more recipes.");

        context.Ingredients.Remove(ingredientToDelete);
        await context.SaveChangesAsync(ct);

        return Result.NoContent(); 
    }
}