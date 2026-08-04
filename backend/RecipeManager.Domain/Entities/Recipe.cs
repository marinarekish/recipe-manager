namespace RecipeManager.Domain.Entities;

public class Recipe
{
    public int RecipeId { get; set; }
    
    public int AuthorId { get; set; }
    public User Author { get; set; } = null!;
    
    public int CuisineId { get; set; }
    public Cuisine Cuisine { get; set; } = null!;
    
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public string Title { get; set; } = null!;
    public int PrepTimeMinutes { get; set; }
    public int CookTimeMinutes { get; set; }
    public int Servings { get; set; }
    
    public string? Instructions { get; set; } 
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
    public ICollection<UserFavorite> UserFavorites { get; set; } = new List<UserFavorite>();
}