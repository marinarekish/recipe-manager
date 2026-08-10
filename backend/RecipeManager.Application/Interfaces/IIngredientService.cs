using RecipeManager.Application.Contracts.Ingredients;
using RecipeManager.Domain.Entities;

namespace RecipeManager.Application.Interfaces;

public interface IIngredientService
{
    Task<List<IngredientResponse>> GetAllIngredientsAsync(CancellationToken ct = default);
    
    Task<IngredientResponse?> GetIngredientByIdAsync(int id, CancellationToken ct = default);
    
    Task<IngredientResponse> CreateIngredientAsync(CreateIngredientRequest ingredient, CancellationToken ct = default);
    Task<IngredientResponse?> UpdateIngredientAsync(int id, CreateIngredientRequest ingredient, CancellationToken ct = default);
    
    Task<bool> DeleteIngredientAsync(int id, CancellationToken ct = default);
}