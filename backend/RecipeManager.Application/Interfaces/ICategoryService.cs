using RecipeManager.Application.Common.Results;
using RecipeManager.Application.Contracts.Categories;

namespace RecipeManager.Application.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryResponse>> GetAllCategoriesAsync(
        CancellationToken ct = default);

    Task<Result<CategoryResponse>> GetCategoryByIdAsync(
        int id,
        CancellationToken ct = default);

    Task<Result<CategoryResponse>> GetOrCreateAsync(
        string name,
        CancellationToken ct = default);
}