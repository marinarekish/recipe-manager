namespace RecipeManager.Application.Contracts.Auth;

public record VerifyLoginCodeRequest(
    string Email,
    string Code
);