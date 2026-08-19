using Microsoft.EntityFrameworkCore;
using NSubstitute;
using RecipeManager.Application.Common.Results;
using RecipeManager.Application.Contracts.Categories;
using RecipeManager.Application.Contracts.Cuisines;
using RecipeManager.Application.Contracts.Ingredients;
using RecipeManager.Application.Contracts.Recipes;
using RecipeManager.Application.Interfaces;
using RecipeManager.Application.Services;
using RecipeManager.Tests.Fixtures;

namespace RecipeManager.Tests.Services;

public class RecipeServiceTests : IDisposable
{
    private readonly Infrastructure.Persistence.ApplicationDbContext _db;
    private readonly RecipeService _sut;
    private readonly ICuisineService _cuisineService;
    private readonly ICategoryService _categoryService;
    private readonly IIngredientService _ingredientService;

    public RecipeServiceTests()
    {
        _db = TestDbContextFactory.Create();
        var mapper = TestMapperFactory.Create();

        _cuisineService = Substitute.For<ICuisineService>();
        _categoryService = Substitute.For<ICategoryService>();
        _ingredientService = Substitute.For<IIngredientService>();

        _sut = new RecipeService(mapper, _db, _cuisineService, _categoryService, _ingredientService);
    }

    public void Dispose() => _db.Dispose();

    private void SetupGetOrCreate()
    {
        _cuisineService.GetOrCreateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(ci => Result<CuisineResponse>.Ok(new CuisineResponse(1, ci.Arg<string>())));
        _categoryService.GetOrCreateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(ci => Result<CategoryResponse>.Ok(new CategoryResponse(1, ci.Arg<string>())));
        _ingredientService.GetOrCreateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(ci => Result<IngredientResponse>.Ok(new IngredientResponse(1, ci.Arg<string>())));
    }

    [Fact]
    public async Task CreateRecipe_ValidRequest_ReturnsOk()
    {
        var (_, _, _, _, _) = await TestDataSeeder.SeedRecipeAsync(_db);
        var author = _db.Users.First();
        SetupGetOrCreate();

        var request = new CreateRecipeRequest(
            Title: "New Recipe",
            CuisineId: null, CuisineName: "mexican",
            CategoryId: null, CategoryName: "lunch",
            PrepTimeMinutes: 5, CookTimeMinutes: 15,
            Servings: 2, Instructions: "Easy.",
            Ingredients: [new RecipeIngredientRequest(null, "salt", 1, "tsp")]);

        var result = await _sut.CreateRecipeAsync(author.UserId, request);

        Assert.Equal(ResultStatus.Ok, result.Status);
        Assert.Equal("New Recipe", result.Value!.Title);
        Assert.Equal(author.UserId, result.Value.AuthorId);
    }

    [Fact]
    public async Task GetRecipeById_ExistingRecipe_ReturnsOk()
    {
        var (_, _, _, _, recipe) = await TestDataSeeder.SeedRecipeAsync(_db);

        var result = await _sut.GetRecipeByIdAsync(recipe.RecipeId);

        Assert.Equal(ResultStatus.Ok, result.Status);
        Assert.Equal("Pasta", result.Value!.Title);
    }

    [Fact]
    public async Task GetRecipeById_NonExistingRecipe_ReturnsNotFound()
    {
        var result = await _sut.GetRecipeByIdAsync(9999);

        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task UpdateRecipe_OwnRecipe_ReturnsOk()
    {
        var (author, _, _, _, recipe) = await TestDataSeeder.SeedRecipeAsync(_db);
        SetupGetOrCreate();

        var request = new UpdateRecipeRequest(
            Title: "Updated Pasta", null, null, null, null,
            null, null, null, null, null);

        var result = await _sut.UpdateRecipeAsync(
            recipe.RecipeId, author.UserId, isAdmin: false, request);

        Assert.Equal(ResultStatus.Ok, result.Status);
        Assert.Equal("Updated Pasta", result.Value!.Title);
    }

    [Fact]
    public async Task UpdateRecipe_NotOwnersRecipe_ReturnsForbidden()
    {
        var (_, _, _, _, recipe) = await TestDataSeeder.SeedRecipeAsync(_db);
        var otherUser = await TestDataSeeder.SeedUserAsync(_db, "other@example.com");

        var request = new UpdateRecipeRequest(
            Title: "Hacked", null, null, null, null,
            null, null, null, null, null);

        var result = await _sut.UpdateRecipeAsync(
            recipe.RecipeId, otherUser.UserId, isAdmin: false, request);

        Assert.Equal(ResultStatus.Forbidden, result.Status);
    }

    [Fact]
    public async Task DeleteRecipe_OwnRecipe_ReturnsOk()
    {
        var (author, _, _, _, recipe) = await TestDataSeeder.SeedRecipeAsync(_db);

        var result = await _sut.DeleteRecipeAsync(
            recipe.RecipeId, author.UserId, isAdmin: false);

        Assert.Equal(ResultStatus.Ok, result.Status);
        Assert.False(await _db.Recipes.AnyAsync(r => r.RecipeId == recipe.RecipeId));
    }

    [Fact]
    public async Task DeleteRecipe_AdminCanDeleteOthersRecipe_ReturnsOk()
    {
        var (author, _, _, _, recipe) = await TestDataSeeder.SeedRecipeAsync(_db);
        var admin = await TestDataSeeder.SeedUserAsync(_db, "admin@example.com", roleId: 1);

        var result = await _sut.DeleteRecipeAsync(
            recipe.RecipeId, admin.UserId, isAdmin: true);

        Assert.Equal(ResultStatus.Ok, result.Status);
        Assert.False(await _db.Recipes.AnyAsync(r => r.RecipeId == recipe.RecipeId));
    }
}
