using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Api.Extensions;
using RecipeManager.Application.Contracts.Favorites;
using RecipeManager.Application.Interfaces;

namespace RecipeManager.Api.Controllers;

[ApiController]
[Route("api/favorites")]
[Authorize]
public class FavoriteController(IFavoriteService favoriteService) : ControllerBase
{
    private int CurrentUserId
    {
        get
        {
            var value =
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub")
                ?? throw new InvalidOperationException("User id claim is missing.");

            return int.Parse(value);
        }
    }

    [HttpGet(Name = "GetFavorites")]
    [ProducesResponseType(typeof(List<FavoriteRecipeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<FavoriteRecipeResponse>>> GetFavorites(
        CancellationToken ct = default)
    {
        var list = await favoriteService.GetUserFavoritesAsync(CurrentUserId, ct);
        return Ok(list);
    }

    [HttpPost(Name = "AddFavorite")]
    [ProducesResponseType(typeof(FavoriteRecipeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddFavorite(
        [FromBody] CreateFavoriteRequest request,
        CancellationToken ct = default)
    {
        var result = await favoriteService.AddFavoriteAsync(
            CurrentUserId, request.RecipeId, ct);

        if (!result.IsSuccess)
            return result.ToActionResult();

        return CreatedAtRoute("GetFavorites", result.Value);
    }

    [HttpDelete("{recipeId:int}", Name = "DeleteFavorite")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteFavorite(
        int recipeId,
        CancellationToken ct = default)
    {
        var result = await favoriteService.RemoveFavoriteAsync(
            CurrentUserId, recipeId, ct);

        return result.IsSuccess
            ? NoContent()
            : result.ToActionResult();
    }
}