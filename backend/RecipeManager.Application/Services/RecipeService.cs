using RecipeManager.Application.Interfaces;
using RecipeManager.Domain.Entities;
using RecipeManager.Infrastructure.Persistence;

namespace RecipeManager.Application.Services;

public class RecipeService(ApplicationDbContext context) : IRecipeService
{
    private readonly ApplicationDbContext _context = context;
    public Task<List<Recipe>> GetAllRecipesAsync(int userId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<Recipe> GetRecipeAsync(int recipeId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<Recipe>> GetAllRecipesByUserIdAsync(int userId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task CreateRecipeAsync(Recipe recipe, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateRecipeAsync(Recipe recipe, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteRecipeAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}