using System.ComponentModel.DataAnnotations;

namespace RecipeManager.Application.Contracts.Cuisines;

public record CreateCuisineRequest(
    [Required, StringLength(50), RegularExpression(@"^.*\S.*$")] string Name
);