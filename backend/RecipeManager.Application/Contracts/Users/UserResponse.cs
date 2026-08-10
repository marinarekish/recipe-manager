using RecipeManager.Application.Contracts.Roles;

namespace RecipeManager.Application.Contracts.Users;

public record UserResponse(
    int UserId,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    List<RoleResponse> Roles,
    DateTime CreatedAt
);