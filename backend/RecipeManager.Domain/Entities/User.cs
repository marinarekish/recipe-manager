namespace RecipeManager.Domain.Entities;

public class User
{
    public int UserId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<Recipe> CreatedRecipes { get; set; }  = new List<Recipe>();
    public ICollection<UserFavorite> UserFavorites { get; set; } = new List<UserFavorite>();
}