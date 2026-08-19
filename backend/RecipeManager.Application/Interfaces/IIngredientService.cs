using RecipeManager.Application.Common.Results;
using RecipeManager.Application.Contracts.Ingredients;

namespace RecipeManager.Application.Interfaces;

public interface IIngredientService
{
    Task<List<IngredientResponse>> GetAllIngredientsAsync(
        CancellationToken ct = default);

    Task<Result<IngredientResponse>> GetIngredientByIdAsync(
        int id, 
        CancellationToken ct = default);

    Task<Result<IngredientResponse>> GetOrCreateAsync(
        string name, 
        CancellationToken ct = default);
    
    // Admin only
    Task<Result> DeleteIngredientAsync(
        int id, 
        CancellationToken ct = default);
}