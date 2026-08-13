using System.ComponentModel.DataAnnotations;

namespace RecipeManager.Application.Contracts.Users;

public record UpdateUserRequest(
    [StringLength(50)] string? FirstName,
    [StringLength(50)] string? LastName,
    [EmailAddress, StringLength(255)] string? Email,
    [StringLength(20)] string? Phone
);
