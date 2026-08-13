using System.ComponentModel.DataAnnotations;

namespace RecipeManager.Application.Contracts.Recipes;

public record RecipeIngredientRequest(
    [Range(1, int.MaxValue)] int? IngredientId,
    [StringLength(50), RegularExpression(@"^.*\S.*$")] string? Name,
    [Range(typeof(decimal), "0.01", "99999999.99")] decimal Amount,
    [Required, StringLength(20)] string Unit
);