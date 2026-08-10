using System.Security.Cryptography;
using System.Text;
using RecipeManager.Application.Interfaces;

namespace RecipeManager.Application.Services;

public class LoginCodeService : ILoginCodeService
{
    public string GenerateCode()
    {
        return RandomNumberGenerator
            .GetInt32(100000, 1000000)
            .ToString();
    }

    public string HashCode(string code)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(code));

        return Convert.ToHexString(bytes);
    }

    public bool VerifyCode(string code, string hash)
    {
        var codeHash = HashCode(code);

        return CryptographicOperations.FixedTimeEquals(
            Convert.FromHexString(codeHash),
            Convert.FromHexString(hash));
    }
}