using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RecipeManager.Application.Common.Results;
using RecipeManager.Application.Contracts.Auth;
using RecipeManager.Application.Interfaces;
using RecipeManager.Domain.Entities;
using RecipeManager.Infrastructure.Persistence;

namespace RecipeManager.Application.Services;

public class AuthService(
    ApplicationDbContext context,
    ILoginCodeService loginCodeService,
    IJwtTokenService jwtTokenService,
    IMapper mapper,
    ILogger<AuthService> logger) : IAuthService
{
    private const int LoginCodeLifetimeMinutes = 10;

    public async Task<Result> RequestLoginCodeAsync(
        string email,
        CancellationToken ct = default)
    {
        email = email.Trim().ToLowerInvariant();

        var user = await context.Users
            .FirstOrDefaultAsync(
                u => u.Email == email,
                ct);

        if (user is null)
            return Result.NotFound("User with this email was not found.");

        var code = loginCodeService.GenerateCode();
        var codeHash = loginCodeService.HashCode(code);

        var now = DateTime.UtcNow;

        var activeTokens = await context.LoginTokens
            .Where(t =>
                t.UserId == user.UserId &&
                t.UsedAt == null &&
                t.ExpiresAt > now)
            .ToListAsync(ct);

        foreach (var token in activeTokens)
        {
            token.UsedAt = now;
        }

        var loginToken = new LoginToken
        {
            UserId = user.UserId,
            CodeHash = codeHash,
            CreatedAt = now,
            ExpiresAt = now.AddMinutes(LoginCodeLifetimeMinutes)
        };

        context.LoginTokens.Add(loginToken);

        await context.SaveChangesAsync(ct);

        // Temporary development behavior.
        // Later this will be replaced by an email sender.
        logger.LogInformation(
            "Login code for user {UserId}: {Code}",
            user.UserId,
            code);

        return Result.Ok();
    }

    public async Task<Result<AuthResponse>> VerifyLoginCodeAsync(
        string email,
        string code,
        CancellationToken ct = default)
    {
        email = email.Trim().ToLowerInvariant();

        var user = await context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(
                u => u.Email == email,
                ct);

        if (user is null)
            return Result<AuthResponse>.NotFound("User with this email was not found.");

        var now = DateTime.UtcNow;

        var loginToken = await context.LoginTokens
            .Where(t =>
                t.UserId == user.UserId &&
                t.UsedAt == null &&
                t.ExpiresAt > now)
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefaultAsync(ct);

        if (loginToken is null)
            return Result<AuthResponse>.Unauthorized("Login code is invalid or expired.");

        if (!loginCodeService.VerifyCode(code, loginToken.CodeHash))
            return Result<AuthResponse>.Unauthorized("Login code is invalid or expired.");

        loginToken.UsedAt = now;

        await context.SaveChangesAsync(ct);

        var userResponse = mapper.Map<Application.Contracts.Users.UserResponse>(user);

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var (token, expiresAt) = jwtTokenService.CreateToken(
            user.UserId, user.Email, roles);

        var expiresIn = (int)(expiresAt - DateTime.UtcNow).TotalSeconds;

        return Result<AuthResponse>.Ok(
            new AuthResponse(userResponse, token, expiresIn));
    }
}