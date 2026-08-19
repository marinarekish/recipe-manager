using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Extensions;
using RecipeManager.Application.Contracts.Roles;
using RecipeManager.Application.Contracts.Users;
using RecipeManager.Application.Interfaces;

namespace RecipeManager.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UserController(IUserService userService) : ControllerBase
{
    private int CurrentUserId
    {
        get
        {
            var value =
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub")
                ?? throw new InvalidOperationException("User id claim is missing.");

            return int.Parse(value);
        }
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrent(CancellationToken ct = default)
    {
        var result = await userService.GetUserByIdAsync(CurrentUserId, ct);
        return result.ToActionResult();
    }

    [HttpPut("me")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCurrentUser(
        [FromBody] UpdateUserRequest userRequest,
        CancellationToken ct = default)
    {
        var result = await userService.UpdateUserAsync(CurrentUserId, userRequest, ct);
        return result.ToActionResult();
    }

    [HttpGet]
    [Authorize(Roles = "Administrator")]
    [ProducesResponseType(typeof(List<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<UserResponse>>> GetAllUsers(CancellationToken ct = default)
    {
        var users = await userService.GetAllUsersAsync(ct);
        return Ok(users);
    }

    [HttpGet("{id:int}", Name = "GetUserById")]
    [Authorize(Roles = "Administrator")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(
        int id,
        CancellationToken ct = default)
    {
        var result = await userService.GetUserByIdAsync(id, ct);
        return result.ToActionResult();
    }

    [HttpPut("{id:int}/role")]
    [Authorize(Roles = "Administrator")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignRole(
        int id,
        [FromBody] AssignRoleRequest roleRequest,
        CancellationToken ct = default)
    {
        var result = await userService.AssignRoleAsync(id, roleRequest.RoleId, ct);
        return result.ToActionResult();
    }
}
