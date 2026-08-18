using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Extensions;
using RecipeManager.Application.Contracts.Roles;
using RecipeManager.Application.Contracts.Users;
using RecipeManager.Application.Interfaces;

namespace RecipeManager.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UserController(IUserService userService) : ControllerBase
{
    // Temporary stubs until JWT is ready
    private const int CurrentUserId = 1; // switch for manual testing
    private const bool IsAdmin = true;   // true = Admin, false = User

    // private const int CurrentUserId = 2; // switch for manual testing
    // private const bool IsAdmin = false;   // true = Admin, false = User

    [HttpGet("me")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrent(CancellationToken ct = default)
    {
        var result = await userService.GetUserByIdAsync(CurrentUserId, ct);
        return result.ToActionResult();
    }

    [HttpPut("me")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCurrentUser(
        [FromBody] UpdateUserRequest userRequest,
        CancellationToken ct = default)
    {
        var result = await userService.UpdateUserAsync(CurrentUserId, userRequest, ct);
        return result.ToActionResult();
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<UserResponse>>> GetAllUsers(CancellationToken ct = default)
    {
        if (!IsAdmin)
            return Forbid();

        var users = await userService.GetAllUsersAsync(ct);
        return Ok(users);
    }

    [HttpGet("{id:int}", Name = "GetUserById")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(
        int id,
        CancellationToken ct = default)
    {
        if (!IsAdmin)
            return Forbid();

        var result = await userService.GetUserByIdAsync(id, ct);
        return result.ToActionResult();
    }

    [HttpPut("{id:int}/role")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignRole(
        int id,
        [FromBody] AssignRoleRequest roleRequest,
        CancellationToken ct = default)
    {
        if (!IsAdmin)
            return Forbid();

        var result = await userService.AssignRoleAsync(id, roleRequest.RoleId, ct);
        return result.ToActionResult();
    }
}
