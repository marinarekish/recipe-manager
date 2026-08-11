using Microsoft.AspNetCore.Mvc;
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
    public async Task<ActionResult<List<RecipeResponse>>> GetAllByUser(CancellationToken ct = default)
    {
        var userRecipes = await recipeService.GetAllRecipesByUserIdAsync(CurrentUserId, ct);
        
        if (userRecipes is null)
            return NotFound();

        return Ok(userRecipes);
    }

    [HttpGet("{id:int}", Name = "GetRecipeById")]
    [ProducesResponseType(typeof(RecipeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecipeResponse>> GetRecipeById(
        int id, 
        CancellationToken ct = default)
    {
        var recipe = await recipeService.GetRecipeByIdAsync(id, ct);
        if (recipe is null)
            return NotFound();
        
        if (!IsAdmin && recipe.AuthorId != CurrentUserId)
            return Forbid();
        
        return Ok(recipe);
    }

    [HttpPost]
    [ProducesResponseType(typeof(RecipeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecipeResponse>> CreateRecipe(
        [FromBody] CreateRecipeRequest recipeRequest,
        CancellationToken ct = default)
    {
        var createdRecipe = await recipeService.CreateRecipeAsync(CurrentUserId, recipeRequest, ct);

        if (createdRecipe is null)
            return NotFound(); 
        
        return CreatedAtRoute("GetRecipeById", new { id = createdRecipe.RecipeId }, createdRecipe);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(RecipeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecipeResponse>> UpdateRecipe(
        int id,
        [FromBody] UpdateRecipeRequest recipeRequest,
        CancellationToken ct = default)
    {
        var recipeToUpdate = await recipeService.GetRecipeByIdAsync(id, ct);
        
        if (recipeToUpdate is null)
            return NotFound();

        if (!IsAdmin && recipeToUpdate.AuthorId != CurrentUserId)
            return Forbid();
        
        var updatedRecipe = await recipeService.UpdateRecipeAsync(id, recipeRequest, ct);
        
        if (updatedRecipe is null)
            return NotFound();

        return Ok(updatedRecipe);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteRecipe(
        int id, 
        CancellationToken ct = default)
    {
        var recipeToDelete = await recipeService.GetRecipeByIdAsync(id, ct);
        if (recipeToDelete is null)
            return NotFound();

        if (!IsAdmin && recipeToDelete.AuthorId != CurrentUserId)
            return Forbid();
        
        var deleted = await recipeService.DeleteRecipeAsync(id, ct);
        
        if (!deleted)
            return NotFound();
        
        return NoContent();
    }
}