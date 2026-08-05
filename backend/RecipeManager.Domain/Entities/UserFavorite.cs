namespace RecipeManager.Domain.Entities;

public class UserFavorite
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}