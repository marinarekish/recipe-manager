using System.ComponentModel.DataAnnotations;

namespace RecipeManager.Application.Contracts.Auth;

public record RequestLoginCodeRequest(
    [property: Required, EmailAddress] string Email
);
