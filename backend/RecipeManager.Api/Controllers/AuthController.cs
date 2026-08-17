using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Extensions;
using RecipeManager.Application.Contracts.Auth;
using RecipeManager.Application.Interfaces;

namespace RecipeManager.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("request-code")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RequestLoginCode(
        [FromBody] RequestLoginCodeRequest request,
        CancellationToken ct = default)
    {
        var result = await authService.RequestLoginCodeAsync(request.Email, ct);

        if (!result.IsSuccess)
            return result.ToActionResult();

        return Ok(new { message = "If the account exists, a login code has been issued." });
    }

    [HttpPost("verify-code")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> VerifyCodeAsync(
        [FromBody] VerifyLoginCodeRequest request, 
        CancellationToken ct = default)
    {
        var result = await authService.VerifyLoginCodeAsync(request.Email, request.Code, ct);
        return result.ToActionResult();
    }
}