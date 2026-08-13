using Microsoft.AspNetCore.Mvc;
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
    public async Task<ActionResult<UserResponse>> GetCurrentUser(CancellationToken ct = default)
    {
        var user = await userService.GetUserByIdAsync(CurrentUserId, ct);

        if (user is null)
            return NotFound();

        return Ok(user);
    }

    [HttpPut("me")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> UpdateCurrentUser(
        [FromBody] UpdateUserRequest userRequest,
        CancellationToken ct = default)
    {
        try
        {
            var result = await userService.UpdateUserAsync(CurrentUserId, userRequest, ct);

            return result.Status switch
            {
                UserOperationStatus.NotFound => NotFound(),
                _ => Ok(result.User)
            };
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
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
    public async Task<ActionResult<UserResponse>> GetUserById(
        int id,
        CancellationToken ct = default)
    {
        if (!IsAdmin)
            return Forbid();

        var user = await userService.GetUserByIdAsync(id, ct);

        if (user is null)
            return NotFound();

        return Ok(user);
    }

    [HttpPut("{id:int}/role")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> AssignRole(
        int id,
        [FromBody] AssignRoleRequest roleRequest,
        CancellationToken ct = default)
    {
        if (!IsAdmin)
            return Forbid();

        try
        {
            var result = await userService.AssignRoleAsync(id, roleRequest.RoleId, ct);

            return result.Status switch
            {
                UserOperationStatus.NotFound => NotFound(),
                _ => Ok(result.User)
            };
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
