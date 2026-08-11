using RecipeManager.Application.Contracts.Cuisines;

namespace RecipeManager.Application.Interfaces;

public interface ICuisineService
{
    Task<List<CuisineResponse>> GetAllCuisinesAsync(
        CancellationToken ct = default);
    
    Task<CuisineResponse?> GetCuisineByIdAsync(
        int id, 
        CancellationToken ct = default);
    
    Task<CuisineResponse?> GetOrCreateAsync(
        string name, 
        CancellationToken ct = default);
}