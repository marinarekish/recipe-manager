using System.ComponentModel.DataAnnotations;

namespace RecipeManager.Application.Contracts.Favorites;

public record CreateFavoriteRequest(
    [property: Range(1, int.MaxValue)] int RecipeId
);
