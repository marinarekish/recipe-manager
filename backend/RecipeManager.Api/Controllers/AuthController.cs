using Microsoft.AspNetCore.Mvc;
using RecipeManager.Application.Contracts.Auth;
using RecipeManager.Application.Interfaces;

namespace RecipeManager.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("request-code")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RequestLoginCode(
        [FromBody] RequestLoginCodeRequest request,
        CancellationToken ct = default)
    {
        try
        {
            await authService.RequestLoginCodeAsync(request.Email, ct);
            return Ok(new { message = "If the account exists, a login code has been issued." });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "User with this email was not found." });
        }
    }

    [HttpPost("verify-code")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    
    public async Task<IActionResult> VerifyCodeAsync(
        [FromBody] VerifyLoginCodeRequest request, 
        CancellationToken ct = default)
    {
        try
        {
            var response = await authService.VerifyLoginCodeAsync(request.Email, request.Code, ct);
            
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { message = "Access denied" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "User with this email was not found." });
        }
    }
}