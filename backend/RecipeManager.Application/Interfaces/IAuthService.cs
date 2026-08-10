using RecipeManager.Application.Contracts.Auth;

namespace RecipeManager.Application.Interfaces;

public interface IAuthService
{
    Task RequestLoginCodeAsync(
        string email,
        CancellationToken ct = default);

    Task<AuthResponse> VerifyLoginCodeAsync(
        string email,
        string code,
        CancellationToken ct = default);
}