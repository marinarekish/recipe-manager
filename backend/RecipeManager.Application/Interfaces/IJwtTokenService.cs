namespace RecipeManager.Application.Interfaces;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAt) CreateToken(
        int userId,
        string email,
        IEnumerable<string> roles);
}
