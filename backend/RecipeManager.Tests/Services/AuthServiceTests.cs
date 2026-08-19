using Microsoft.Extensions.Logging;
using NSubstitute;
using RecipeManager.Application.Common.Results;
using RecipeManager.Application.Interfaces;
using RecipeManager.Application.Services;
using RecipeManager.Domain.Entities;
using RecipeManager.Infrastructure.Persistence;
using RecipeManager.Tests.Fixtures;

namespace RecipeManager.Tests.Services;

public class AuthServiceTests : IDisposable
{
    private readonly ApplicationDbContext _db;
    private readonly AuthService _sut;
    private readonly ILoginCodeService _loginCodeService;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthServiceTests()
    {
        _db = TestDbContextFactory.Create();
        _loginCodeService = new LoginCodeService();
        _jwtTokenService = Substitute.For<IJwtTokenService>();

        var mapper = TestMapperFactory.Create();
        var logger = Substitute.For<ILogger<AuthService>>();

        _sut = new AuthService(_db, _loginCodeService, _jwtTokenService, mapper, logger);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task RequestCode_ValidUser_ReturnsOk()
    {
        var user = await TestDataSeeder.SeedUserAsync(_db);

        var result = await _sut.RequestLoginCodeAsync(user.Email);

        Assert.Equal(ResultStatus.Ok, result.Status);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public async Task RequestCode_UserNotFound_ReturnsNotFound()
    {
        var result = await _sut.RequestLoginCodeAsync("nobody@example.com");

        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task VerifyCode_ValidCode_ReturnsOkWithToken()
    {
        var user = await TestDataSeeder.SeedUserAsync(_db, roleId: 1);
        var code = "123456";
        var hash = _loginCodeService.HashCode(code);

        _db.LoginTokens.Add(new LoginToken
        {
            UserId = user.UserId,
            CodeHash = hash,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10)
        });
        await _db.SaveChangesAsync();

        _jwtTokenService
            .CreateToken(user.UserId, user.Email, Arg.Any<List<string>>())
            .Returns(("fake-jwt-token", DateTime.UtcNow.AddHours(1)));

        var result = await _sut.VerifyLoginCodeAsync(user.Email, code);

        Assert.Equal(ResultStatus.Ok, result.Status);
        var response = result.Value!;
        Assert.Equal("fake-jwt-token", response.AccessToken);
        Assert.True(response.ExpiresIn > 0);
        Assert.Equal(user.UserId, response.User.UserId);
        Assert.Contains(response.User.Roles, r => r.Name == "Administrator");
    }

    [Fact]
    public async Task VerifyCode_InvalidCode_ReturnsUnauthorized()
    {
        var user = await TestDataSeeder.SeedUserAsync(_db);
        var correctCode = "123456";
        var hash = _loginCodeService.HashCode(correctCode);

        _db.LoginTokens.Add(new LoginToken
        {
            UserId = user.UserId,
            CodeHash = hash,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10)
        });
        await _db.SaveChangesAsync();

        var result = await _sut.VerifyLoginCodeAsync(user.Email, "000000");

        Assert.Equal(ResultStatus.Unauthorized, result.Status);
    }

    [Fact]
    public async Task VerifyCode_ExpiredCode_ReturnsUnauthorized()
    {
        var user = await TestDataSeeder.SeedUserAsync(_db);
        var code = "123456";
        var hash = _loginCodeService.HashCode(code);

        _db.LoginTokens.Add(new LoginToken
        {
            UserId = user.UserId,
            CodeHash = hash,
            CreatedAt = DateTime.UtcNow.AddMinutes(-20),
            ExpiresAt = DateTime.UtcNow.AddMinutes(-10)
        });
        await _db.SaveChangesAsync();

        var result = await _sut.VerifyLoginCodeAsync(user.Email, code);

        Assert.Equal(ResultStatus.Unauthorized, result.Status);
    }
}
