using System.ComponentModel.DataAnnotations;

namespace RecipeManager.Application.Contracts.Ingredients;

public record CreateIngredientRequest(
    [Required, StringLength(50), RegularExpression(@"^.*\S.*$")] string Name
);