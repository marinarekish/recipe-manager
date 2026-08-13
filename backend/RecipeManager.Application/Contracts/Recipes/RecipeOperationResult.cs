namespace RecipeManager.Application.Contracts.Recipes;

public enum RecipeOperationStatus
{
    Ok,
    NotFound,
    Forbidden
}

public record RecipeUpdateResult(
    RecipeOperationStatus Status,
    RecipeResponse? Recipe
);
