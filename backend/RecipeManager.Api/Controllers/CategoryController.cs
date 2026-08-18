using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Extensions;
using RecipeManager.Application.Contracts.Categories;
using RecipeManager.Application.Interfaces;

namespace RecipeManager.Api.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoryController(
    ICategoryService categoryService) : ControllerBase
{
    [HttpGet(Name = "GetAllCategories")]
    public async Task<ActionResult<List<CategoryResponse>>> GetAllCategories(CancellationToken ct = default)
    {
        return await categoryService.GetAllCategoriesAsync(ct);
    }

    [HttpGet("{id}", Name = "GetCategoryById")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategoryById(int id, CancellationToken ct = default)
    {
        var result = await categoryService.GetCategoryByIdAsync(id, ct);
        return result.ToActionResult();
    }

    [HttpPost("get-or-create", Name = "GetOrCreateCategory")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrCreate(
        [FromBody] CreateCategoryRequest request,
        CancellationToken ct = default)
    {
        var result = await categoryService.GetOrCreateAsync(request.Name, ct);
        return result.ToActionResult();
    }

    // Update and Delete are out of scope.
    // Users cannot modify or remove shared reference data.
}