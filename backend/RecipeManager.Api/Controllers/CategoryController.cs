using Microsoft.AspNetCore.Mvc;
using RecipeManager.Application.Contracts.Categories;
using RecipeManager.Application.Interfaces;

namespace RecipeManager.Api.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoryController (
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
    public async Task<ActionResult<CategoryResponse>> GetCategoryById(int id, CancellationToken ct = default)
    {
        var category = await categoryService.GetCategoryByIdAsync(id, ct);
        
        if  (category == null)
            return NotFound();
        
        return category;
    }

    [HttpPost("get-or-create", Name = "GetOrCreateCategory")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CategoryResponse>> GetOrCreate(
        [FromBody] CreateCategoryRequest request,
        CancellationToken ct = default)
    {
        try
        {
            var category = await categoryService.GetOrCreateAsync(request.Name, ct);
            
            if (category == null)
                return BadRequest();
            
            return category;
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    // Update and Delete are out of scope.
    // Users cannot modify or remove shared reference data.
}