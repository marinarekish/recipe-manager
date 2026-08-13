using System.ComponentModel.DataAnnotations;

namespace RecipeManager.Application.Contracts.Users;

public record CreateUserRequest(
    [property: Required, StringLength(50)] string FirstName,
    [property: Required, StringLength(50)] string LastName,
    [property: Required, EmailAddress, StringLength(255)] string Email,
    [property: StringLength(20)] string? Phone
);
