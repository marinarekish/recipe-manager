using RecipeManager.Application.Interfaces;
using RecipeManager.Domain.Entities;
using RecipeManager.Infrastructure.Persistence;

namespace RecipeManager.Application.Services;

public class IngredientService(ApplicationDbContext context) : IIngredientService
{
    private readonly ApplicationDbContext _context = context;
    public Task<List<Ingredient>> GetAllIngredientsAsync(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<Ingredient?> GetIngredientByIdAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<Ingredient> CreateIngredientAsync(Ingredient ingredient, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateIngredientAsync(Ingredient? ingredient, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteIngredientAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}