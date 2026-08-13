using System.ComponentModel.DataAnnotations;

namespace RecipeManager.Application.Contracts.Auth;

public record RequestLoginCodeRequest(
    [Required, EmailAddress] string Email
);
