using Microsoft.AspNetCore.Mvc;
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
    public async Task<ActionResult<FavoriteRecipeResponse?>> AddFavoriteAsync(
        [FromBody] CreateFavoriteRequest request,
        CancellationToken ct = default)
    {
        var created = await favoriteService.AddFavoriteAsync(
            CurrentUserId, request.RecipeId, ct);

        if (created is not null)
            return CreatedAtRoute("GetFavorites", created);

        if (await favoriteService.IsFavoriteAsync(CurrentUserId, request.RecipeId, ct))
            return Conflict(new { message = "Recipe is already in favorites." });

        return NotFound(new { message = "Recipe not found." });
    }

    [HttpDelete("{id:int}", Name = "DeleteFavorite")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteFavoriteAsync(int id, CancellationToken ct = default)
    {
        var favToDelete = await favoriteService.RemoveFavoriteAsync(CurrentUserId, id, ct);
        
        return favToDelete ? NoContent() : NotFound();
    }
}