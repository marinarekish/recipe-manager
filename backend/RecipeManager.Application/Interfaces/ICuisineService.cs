using RecipeManager.Application.Common.Results;
using RecipeManager.Application.Contracts.Cuisines;

namespace RecipeManager.Application.Interfaces;

public interface ICuisineService
{
    Task<List<CuisineResponse>> GetAllCuisinesAsync(
        CancellationToken ct = default);

    Task<Result<CuisineResponse>> GetCuisineByIdAsync(
        int id,
        CancellationToken ct = default);

    Task<Result<CuisineResponse>> GetOrCreateAsync(
        string name,
        CancellationToken ct = default);
    
    // Admin only
    Task<Result> DeleteCuisineAsync(
        int id, 
        CancellationToken ct = default);
}