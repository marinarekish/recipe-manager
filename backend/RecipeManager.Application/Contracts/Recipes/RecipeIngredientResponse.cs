namespace RecipeManager.Application.Contracts.Recipes;

public class RecipeIngredientResponse
{
    public int IngredientId { get; init; }
    public string Name { get; init; } = null!;
    public decimal Amount { get; init; }
    public string Unit { get; init; } = null!;
}