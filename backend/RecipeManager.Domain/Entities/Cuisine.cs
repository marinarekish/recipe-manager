namespace RecipeManager.Domain.Entities;

public class Cuisine
{
    public int CuisineId { get; set; }
    public string Name { get; set; } = null!;
    
    public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
}