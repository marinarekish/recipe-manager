using Microsoft.AspNetCore.Mvc;
using RecipeManager.Application.Contracts.Ingredients;
using RecipeManager.Application.Interfaces;

namespace RecipeManager.Api.Controllers;

[ApiController]
[Route("api/ingredients")]
public class IngredientController(
    IIngredientService ingredientService) : ControllerBase
{
    [HttpGet(Name = "GetAllIngredients")]
    public async Task<ActionResult<List<IngredientResponse>>> GetAllIngredients(CancellationToken ct = default)
    {
        return await ingredientService.GetAllIngredientsAsync(ct);
    }

    [HttpGet("{id}", Name = "GetIngredientById")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(IngredientResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<IngredientResponse>> GetIngredientById(int id, CancellationToken ct = default)
    {
        var ingredient = await ingredientService.GetIngredientByIdAsync(id, ct);

        if (ingredient == null)
            return NotFound();
        
        return ingredient;
    }
    
    [HttpPost("get-or-create", Name = "GetOrCreateIngredient")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IngredientResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<IngredientResponse>> GetOrCreateIngredient(
        [FromBody] CreateIngredientRequest request,
        CancellationToken ct = default)
    {
        try
        {
            var ingredient = await ingredientService.GetOrCreateAsync(request.Name, ct);

            if (ingredient == null)
                return BadRequest();

            return ingredient;

        } catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    // Update and Delete are out of scope.
    // Users cannot modify or remove shared reference data.
}