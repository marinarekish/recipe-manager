using RecipeManager.Application.Contracts.Recipes;

namespace RecipeManager.Application.Interfaces;

public interface IRecipeService
{
    Task<List<RecipeResponse>> GetAllRecipesByAdminAsync(
        CancellationToken ct = default);

    Task<RecipeResponse?> GetRecipeByIdAsync(
        int recipeId,
        CancellationToken ct = default);

    Task<List<RecipeResponse>?> GetAllRecipesByUserIdAsync(
        int authorId,
        CancellationToken ct = default);

    Task<RecipeResponse?> CreateRecipeAsync(
        int authorId,
        CreateRecipeRequest recipe,
        CancellationToken ct = default);

    Task<RecipeUpdateResult> UpdateRecipeAsync(
        int recipeId,
        int currentUserId,
        bool isAdmin,
        UpdateRecipeRequest recipe,
        CancellationToken ct = default);

    Task<RecipeOperationStatus> DeleteRecipeAsync(
        int recipeId,
        int currentUserId,
        bool isAdmin,
        CancellationToken ct = default);
}