using System.ComponentModel.DataAnnotations;

namespace RecipeManager.Application.Contracts.Roles;

public record AssignRoleRequest(
    [property: Range(1, int.MaxValue)] int UserId,
    [property: Range(1, int.MaxValue)] int RoleId
);
