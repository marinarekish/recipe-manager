using RecipeManager.Application.Common.Results;
using RecipeManager.Application.Services;
using RecipeManager.Tests.Fixtures;

namespace RecipeManager.Tests.Services;

public class FavoriteServiceTests : IDisposable
{
    private readonly Infrastructure.Persistence.ApplicationDbContext _db;
    private readonly FavoriteService _sut;

    public FavoriteServiceTests()
    {
        _db = TestDbContextFactory.Create();
        var mapper = TestMapperFactory.Create();
        _sut = new FavoriteService(mapper, _db);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task AddFavorite_ValidRecipe_ReturnsOk()
    {
        var (author, _, _, _, recipe) = await TestDataSeeder.SeedRecipeAsync(_db);
        var user = await TestDataSeeder.SeedUserAsync(_db, "fan@example.com");

        var result = await _sut.AddFavoriteAsync(user.UserId, recipe.RecipeId);

        Assert.Equal(ResultStatus.Ok, result.Status);
        Assert.Equal(recipe.RecipeId, result.Value!.RecipeId);
        Assert.Equal("Pasta", result.Value.Title);
    }

    [Fact]
    public async Task AddFavorite_AlreadyFavorited_ReturnsConflict()
    {
        var (author, _, _, _, recipe) = await TestDataSeeder.SeedRecipeAsync(_db);
        var user = await TestDataSeeder.SeedUserAsync(_db, "fan@example.com");

        await _sut.AddFavoriteAsync(user.UserId, recipe.RecipeId);
        var result = await _sut.AddFavoriteAsync(user.UserId, recipe.RecipeId);

        Assert.Equal(ResultStatus.Conflict, result.Status);
    }

    [Fact]
    public async Task RemoveFavorite_ExistingFavorite_ReturnsOk()
    {
        var (author, _, _, _, recipe) = await TestDataSeeder.SeedRecipeAsync(_db);
        var user = await TestDataSeeder.SeedUserAsync(_db, "fan@example.com");

        await _sut.AddFavoriteAsync(user.UserId, recipe.RecipeId);
        var result = await _sut.RemoveFavoriteAsync(user.UserId, recipe.RecipeId);

        Assert.Equal(ResultStatus.Ok, result.Status);
        Assert.False(await _sut.IsFavoriteAsync(user.UserId, recipe.RecipeId));
    }

    [Fact]
    public async Task GetUserFavorites_HasFavorites_ReturnsList()
    {
        var (author, _, _, _, recipe) = await TestDataSeeder.SeedRecipeAsync(_db);
        var user = await TestDataSeeder.SeedUserAsync(_db, "fan@example.com");

        await _sut.AddFavoriteAsync(user.UserId, recipe.RecipeId);
        var favorites = await _sut.GetUserFavoritesAsync(user.UserId);

        Assert.Single(favorites);
        Assert.Equal("Pasta", favorites[0].Title);
    }
}
