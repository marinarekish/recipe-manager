using System.ComponentModel.DataAnnotations;

namespace RecipeManager.Application.Contracts.Users;

public record CreateUserRequest(
    [Required, StringLength(50)] string FirstName,
    [Required, StringLength(50)] string LastName,
    [Required, EmailAddress, StringLength(255)] string Email,
    [StringLength(20)] string? Phone
);
