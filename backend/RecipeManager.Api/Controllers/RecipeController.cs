using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Extensions;
using RecipeManager.Application.Contracts.Recipes;
using RecipeManager.Application.Interfaces;

namespace RecipeManager.Api.Controllers;

[ApiController]
[Route("api/recipes")]
[Authorize]
public class RecipeController(
    IRecipeService recipeService) : ControllerBase
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

    private bool IsAdmin => User.IsInRole("Administrator");

    [HttpGet]
    [ProducesResponseType(typeof(List<RecipeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<RecipeResponse>>> GetAll(CancellationToken ct = default)
    {
        var recipes = await recipeService.GetAllRecipesAsync(ct);
        return Ok(recipes);
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(List<RecipeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllByUser(CancellationToken ct = default)
    {
        var result = await recipeService.GetAllRecipesByUserIdAsync(CurrentUserId, ct);
        return result.ToActionResult();
    }

    [HttpGet("{id:int}", Name = "GetRecipeById")]
    [ProducesResponseType(typeof(RecipeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRecipeById(
        int id,
        CancellationToken ct = default)
    {
        var result = await recipeService.GetRecipeByIdAsync(id, ct);
        return result.ToActionResult();
    }

    [HttpPost]
    [ProducesResponseType(typeof(RecipeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateRecipe(
        [FromBody] CreateRecipeRequest recipeRequest,
        CancellationToken ct = default)
    {
        var result = await recipeService.CreateRecipeAsync(CurrentUserId, recipeRequest, ct);

        if (!result.IsSuccess)
            return result.ToActionResult();

        return CreatedAtRoute("GetRecipeById", new { id = result.Value!.RecipeId }, result.Value);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(RecipeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRecipe(
        int id,
        [FromBody] UpdateRecipeRequest recipeRequest,
        CancellationToken ct = default)
    {
        var result = await recipeService.UpdateRecipeAsync(
            id, CurrentUserId, IsAdmin, recipeRequest, ct);

        return result.ToActionResult();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRecipe(
        int id,
        CancellationToken ct = default)
    {
        var result = await recipeService.DeleteRecipeAsync(id, CurrentUserId, IsAdmin, ct);
        return result.ToActionResult();
    }
}
