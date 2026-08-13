using System.ComponentModel.DataAnnotations;

namespace RecipeManager.Application.Contracts.Roles;

public record AssignRoleRequest(
    [Range(1, int.MaxValue)] int RoleId
);
