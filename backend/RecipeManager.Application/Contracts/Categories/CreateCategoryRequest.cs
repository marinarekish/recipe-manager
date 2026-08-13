using System.ComponentModel.DataAnnotations;

namespace RecipeManager.Application.Contracts.Categories;

public record CreateCategoryRequest(
    [Required, StringLength(50), RegularExpression(@"^.*\S.*$")] string Name
);