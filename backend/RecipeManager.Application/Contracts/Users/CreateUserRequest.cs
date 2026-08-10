namespace RecipeManager.Application.Contracts.Users;

public record CreateUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string? Phone
    );