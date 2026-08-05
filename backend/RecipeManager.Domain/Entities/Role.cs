namespace RecipeManager.Domain.Entities;

public class Role
{
    public int RoleId { get; set; }
    public string Name { get; set; } = null!;
    
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}