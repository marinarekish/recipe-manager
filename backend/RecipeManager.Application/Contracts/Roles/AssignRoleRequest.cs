namespace RecipeManager.Application.Contracts.Roles;

public record AssignRoleRequest(
    int UserId,
    int RoleId
);