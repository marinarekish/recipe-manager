using System.ComponentModel.DataAnnotations;

namespace RecipeManager.Application.Contracts.Categories;

public record CreateCategoryRequest(
    [property: Required, StringLength(50), RegularExpression(@"\S")] string Name
);
