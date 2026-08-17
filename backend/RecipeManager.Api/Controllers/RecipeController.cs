using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Extensions;
using RecipeManager.Application.Common.Results;
using RecipeManager.Application.Contracts.Recipes;
using RecipeManager.Application.Interfaces;

namespace RecipeManager.Api.Controllers;

[ApiController]
[Route("api/recipes")]
public class RecipeController (
    IRecipeService recipeService) : ControllerBase
{
    // Temporary stubs until JWT is ready
    private const int CurrentUserId = 1; // switch for manual testing
    private const bool IsAdmin = true;   // true = Admin, false = User
    
    // private const int CurrentUserId = 2; // switch for manual testing
    // private const bool IsAdmin = false;   // true = Admin, false = User

    [HttpGet]
    [ProducesResponseType(typeof(List<RecipeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<RecipeResponse>>> GetAllByAdmin(CancellationToken ct = default)
    {
        if (!IsAdmin)
            return Forbid();

        var recipes = await recipeService.GetAllRecipesByAdminAsync(ct);
        return Ok(recipes);
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(List<RecipeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllByUser(CancellationToken ct = default)
    {
        var result = await recipeService.GetAllRecipesByUserIdAsync(CurrentUserId, ct);
        return result.ToActionResult();
    }

    [HttpGet("{id:int}", Name = "GetRecipeById")]
    [ProducesResponseType(typeof(RecipeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRecipeById(
        int id, 
        CancellationToken ct = default)
    {
        var result = await recipeService.GetRecipeByIdAsync(id, ct);

        if (!result.IsSuccess)
            return result.ToActionResult();

        if (!IsAdmin && result.Value!.AuthorId != CurrentUserId)
            return Forbid();

        return Ok(result.Value);
    }

    [HttpPost]
    [ProducesResponseType(typeof(RecipeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRecipe(
        int id,
        CancellationToken ct = default)
    {
        var result = await recipeService.DeleteRecipeAsync(id, CurrentUserId, IsAdmin, ct);
        return result.IsSuccess ? NoContent() : result.ToActionResult();
    }
}