using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
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

    public async Task<IngredientResponse?> GetIngredientByIdAsync(
        int id, 
        CancellationToken ct = default)
    {
        return await context.Ingredients
            .AsNoTracking()
            .Where(i => i.IngredientId == id)
            .ProjectTo<IngredientResponse>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IngredientResponse> CreateIngredientAsync(
        CreateIngredientRequest ingredient, 
        CancellationToken ct = default)
    {
        var ingredientToCreate = mapper.Map<Ingredient>(ingredient);
        
        context.Ingredients.Add(ingredientToCreate);
        await context.SaveChangesAsync(ct);
        
        return mapper.Map<IngredientResponse>(ingredientToCreate);
    }

    public async Task<IngredientResponse?> UpdateIngredientAsync(
        int id, 
        CreateIngredientRequest ingredient, 
        CancellationToken ct = default)
    {
        var ingredientToUpdate = await context.Ingredients
            .FirstOrDefaultAsync(i => i.IngredientId == id, ct);
            
        if (ingredientToUpdate == null)
            return null;
        
        mapper.Map(ingredient, ingredientToUpdate);
        
        await context.SaveChangesAsync(ct);
        
        return mapper.Map<IngredientResponse>(ingredientToUpdate);
    }

    public async Task<bool> DeleteIngredientAsync(
        int id, 
        CancellationToken ct = default)
    {
        var ingredientToDelete = await context.Ingredients.FirstOrDefaultAsync(i => i.IngredientId == id, cancellationToken: ct);
        
        if (ingredientToDelete == null)
            return false;
        
        context.Ingredients.Remove(ingredientToDelete);
        
        await context.SaveChangesAsync(ct);
        return true;
    }
}