using RecipeManager.Domain.Entities;

namespace RecipeManager.Application.Interfaces;

public interface IIngredientService
{
    Task<List<Ingredient>> GetAllIngredientsAsync(CancellationToken ct = default);
    
    Task<Ingredient?> GetIngredientByIdAsync(int id, CancellationToken ct = default);
    
    Task<Ingredient> CreateIngredientAsync(Ingredient ingredient, CancellationToken ct = default);
    Task UpdateIngredientAsync(Ingredient? ingredient, CancellationToken ct = default);
    
    Task<bool> DeleteIngredientAsync(int id, CancellationToken ct = default);
}