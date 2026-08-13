using System.ComponentModel.DataAnnotations;

namespace RecipeManager.Application.Contracts.Auth;

public record VerifyLoginCodeRequest(
    [property: Required, EmailAddress] string Email,
    [property: Required, StringLength(6, MinimumLength = 6)] string Code
);
