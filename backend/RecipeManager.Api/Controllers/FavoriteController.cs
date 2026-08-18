using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Extensions;
using RecipeManager.Application.Contracts.Favorites;
using RecipeManager.Application.Interfaces;

namespace RecipeManager.Api.Controllers;

[ApiController]
[Route("api/favorites")]
public class FavoriteController(IFavoriteService favoriteService) : ControllerBase
{
    // Temporary stubs until JWT is ready
    private const int CurrentUserId = 1;
    
    [HttpGet(Name = "GetFavorites")]
    [ProducesResponseType(typeof(List<FavoriteRecipeResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<FavoriteRecipeResponse>>> GetFavorites(
        CancellationToken ct = default)
    {
        var list = await favoriteService.GetUserFavoritesAsync(CurrentUserId, ct);
        return Ok(list);
    }

    [HttpPost(Name = "AddFavorite")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddFavoriteAsync(
        [FromBody] CreateFavoriteRequest request,
        CancellationToken ct = default)
    {
        var result = await favoriteService.AddFavoriteAsync(
            CurrentUserId, request.RecipeId, ct);

        if (!result.IsSuccess)
            return result.ToActionResult();

        return CreatedAtRoute("GetFavorites", result.Value);
    }

    [HttpDelete("{id:int}", Name = "DeleteFavorite")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteFavoriteAsync(int id, CancellationToken ct = default)
    {
        var result = await favoriteService.RemoveFavoriteAsync(CurrentUserId, id, ct);
        return result.IsSuccess ? NoContent() : result.ToActionResult();
    }
}