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

    public async Task<IngredientResponse?> GetOrCreateAsync(string name, CancellationToken ct = default)
    {
        var trimmedName = name.Trim();

        if (string.IsNullOrWhiteSpace(trimmedName))
            throw new ArgumentException("Ingredient name cannot be empty.", nameof(name));

        var ingredient = await context.Ingredients
            .FirstOrDefaultAsync(i => i.Name.ToLower() == trimmedName.ToLower(), cancellationToken: ct);

        if (ingredient != null)
            return mapper.Map<IngredientResponse>(ingredient);

        ingredient = new Ingredient
        {
            Name = trimmedName
        };

        context.Ingredients.Add(ingredient);

        try
        {
            await context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            ingredient = await context.Ingredients
                .FirstAsync(i => i.Name.ToLower() == trimmedName.ToLower(), cancellationToken: ct);
        }
        
        return mapper.Map<IngredientResponse>(ingredient);
    }
}