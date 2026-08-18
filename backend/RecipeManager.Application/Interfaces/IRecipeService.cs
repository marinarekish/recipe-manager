using RecipeManager.Application.Common.Results;
using RecipeManager.Application.Contracts.Recipes;

namespace RecipeManager.Application.Interfaces;

public interface IRecipeService
{
    Task<List<RecipeResponse>> GetAllRecipesByAdminAsync(
        CancellationToken ct = default);

    Task<Result<RecipeResponse>> GetRecipeByIdAsync(
        int recipeId,
        CancellationToken ct = default);

    Task<Result<List<RecipeResponse>>> GetAllRecipesByUserIdAsync(
        int authorId,
        CancellationToken ct = default);

    Task<Result<RecipeResponse>> CreateRecipeAsync(
        int authorId,
        CreateRecipeRequest recipe,
        CancellationToken ct = default);

    Task<Result<RecipeResponse>> UpdateRecipeAsync(
        int recipeId,
        int currentUserId,
        bool isAdmin,
        UpdateRecipeRequest recipe,
        CancellationToken ct = default);

    Task<Result> DeleteRecipeAsync(
        int recipeId,
        int currentUserId,
        bool isAdmin,
        CancellationToken ct = default);
}