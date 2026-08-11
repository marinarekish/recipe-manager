using Microsoft.AspNetCore.Mvc;
using RecipeManager.Application.Contracts.Cuisines;
using RecipeManager.Application.Interfaces;

namespace RecipeManager.Api.Controllers;

[ApiController]
[Route("api/cuisines")]
public class CuisineController (
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
    public async Task<ActionResult<CuisineResponse>> GetCuisineById(int id, CancellationToken ct = default)
    {
        var cuisine = await cuisineService.GetCuisineByIdAsync(id, ct);

        if (cuisine == null)
            return NotFound();
        
        return cuisine;
    }

    [HttpPost("get-or-create", Name = "GetOrCreateCuisine")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CuisineResponse),StatusCodes.Status200OK)]
    public async Task<ActionResult<CuisineResponse>> GetOrCreateCuisine(
        [FromBody] CreateCuisineRequest request,
        CancellationToken ct = default)
    {
        try
        {
            var cuisine = await cuisineService.GetOrCreateAsync(request.Name, ct);
            
            if (cuisine == null)
                return BadRequest();

            return cuisine;

        } catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    // Update and Delete are out of scope.
    // Users cannot modify or remove shared reference data.
}