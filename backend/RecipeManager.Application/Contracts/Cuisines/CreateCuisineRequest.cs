using System.ComponentModel.DataAnnotations;

namespace RecipeManager.Application.Contracts.Cuisines;

public record CreateCuisineRequest(
    [property: Required, StringLength(50), RegularExpression(@"\S")] string Name
);
