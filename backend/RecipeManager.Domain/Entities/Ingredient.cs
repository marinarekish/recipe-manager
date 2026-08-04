namespace RecipeManager.Domain.Entities;

public class Ingredient
{
    public int IngredientId { get; set; }
    public string Name { get; set; } = null!;
    
    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
}