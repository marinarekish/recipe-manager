using RecipeManager.Application.Contracts.Roles;

namespace RecipeManager.Application.Contracts.Users;

public class UserResponse
{
    public int UserId { get; init; }
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string? Phone { get; init; }
    public List<RoleResponse> Roles { get; init; } = new();
    public DateTime CreatedAt { get; init; }
}
