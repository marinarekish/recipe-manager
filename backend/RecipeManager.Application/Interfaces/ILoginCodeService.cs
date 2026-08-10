namespace RecipeManager.Application.Interfaces;

public interface ILoginCodeService
{
    string GenerateCode();

    string HashCode(string code);

    bool VerifyCode(string code, string hash);
}