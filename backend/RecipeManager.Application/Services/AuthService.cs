using RecipeManager.Application.Contracts.Auth;
using RecipeManager.Application.Interfaces;
using RecipeManager.Infrastructure.Persistence;

namespace RecipeManager.Application.Services;

public class AuthService(ApplicationDbContext context) : IAuthService
{
    private readonly ApplicationDbContext _context = context;

    public Task RequestLoginCodeAsync(
        string email,
        CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<AuthResponse> VerifyLoginCodeAsync(
        string email,
        string code,
        CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}