namespace RecipeManager.Application.Contracts.Users;

using RecipeManager.Application.Contracts.Roles;

public record UserResponse(
    int UserId,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    List<RoleResponse> Roles, 
    DateTime CreatedAt
);