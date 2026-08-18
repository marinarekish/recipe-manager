using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Extensions;
using RecipeManager.Application.Contracts.Cuisines;
using RecipeManager.Application.Interfaces;

namespace RecipeManager.Api.Controllers;

[ApiController]
[Route("api/cuisines")]
public class CuisineController(
    ICuisineService cuisineService) : ControllerBase
{
    [HttpGet(Name = "GetAllCuisines")]
    public async Task<ActionResult<List<CuisineResponse>>> GetAllCuisines(CancellationToken ct = default)
    {
        return await cuisineService.GetAllCuisinesAsync(ct);
    }

    [HttpGet("{id}", Name = "GetCuisineById")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(CuisineResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCuisineById(int id, CancellationToken ct = default)
    {
        var result = await cuisineService.GetCuisineByIdAsync(id, ct);
        return result.ToActionResult();
    }

    [HttpPost("get-or-create", Name = "GetOrCreateCuisine")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CuisineResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrCreateCuisine(
        [FromBody] CreateCuisineRequest request,
        CancellationToken ct = default)
    {
        var result = await cuisineService.GetOrCreateAsync(request.Name, ct);
        return result.ToActionResult();
    }

    // Update and Delete are out of scope.
    // Users cannot modify or remove shared reference data.
}