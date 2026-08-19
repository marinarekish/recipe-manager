using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Extensions;
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
    public async Task<IActionResult> GetIngredientById(int id, CancellationToken ct = default)
    {
        var result = await ingredientService.GetIngredientByIdAsync(id, ct);
        return result.ToActionResult();
    }

    [HttpPost("get-or-create", Name = "GetOrCreateIngredient")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IngredientResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrCreateIngredient(
        [FromBody] CreateIngredientRequest request,
        CancellationToken ct = default)
    {
        var result = await ingredientService.GetOrCreateAsync(request.Name, ct);
        return result.ToActionResult();
    }

    [HttpDelete("{id}", Name = "DeleteIngredient")]
    [Authorize(Roles = "Administrator")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteIngredient(
        int id,
        CancellationToken ct = default)
    {
        var result = await ingredientService.DeleteIngredientAsync(id, ct);
        return result.ToActionResult();
    }
}