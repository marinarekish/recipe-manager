using RecipeManager.Domain.Entities;

namespace RecipeManager.Application.Interfaces;

public interface IRecipeService
{
    Task<List<Recipe>> GetAllRecipesAsync(int userId, CancellationToken ct = default);
    
    Task<Recipe> GetRecipeAsync(int recipeId, CancellationToken ct = default);
    Task<List<Recipe>> GetAllRecipesByUserIdAsync(int userId, CancellationToken ct = default);
    
    Task CreateRecipeAsync(Recipe recipe, CancellationToken ct = default);
    Task UpdateRecipeAsync(Recipe recipe, CancellationToken ct = default);
    
    Task<bool> DeleteRecipeAsync(int id, CancellationToken ct = default);
    
}