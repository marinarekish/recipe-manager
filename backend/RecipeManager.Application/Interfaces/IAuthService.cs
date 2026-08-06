namespace RecipeManager.Application.Interfaces;

public interface IAuthService
{
    Task RequestLoginCodeAsync(string email, CancellationToken ct = default);

    Task<bool> VerifyLoginCodeAsync(
        string email,
        string code,
        CancellationToken ct = default);
}