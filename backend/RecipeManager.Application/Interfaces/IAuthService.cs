using RecipeManager.Application.Common.Results;
using RecipeManager.Application.Contracts.Auth;

namespace RecipeManager.Application.Interfaces;

public interface IAuthService
{
    Task<Result> RequestLoginCodeAsync(
        string email,
        CancellationToken ct = default);

    Task<Result<AuthResponse>> VerifyLoginCodeAsync(
        string email,
        string code,
        CancellationToken ct = default);
}