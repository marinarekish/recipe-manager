using RecipeManager.Application.Contracts.Users;

namespace RecipeManager.Application.Contracts.Auth;

public record AuthResponse(
    UserResponse User
);