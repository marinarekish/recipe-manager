using System.ComponentModel.DataAnnotations;

namespace RecipeManager.Application.Contracts.Auth;

public record VerifyLoginCodeRequest(
    [Required, EmailAddress] string Email,
    [Required, StringLength(6, MinimumLength = 6)] string Code
);
