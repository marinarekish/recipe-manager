using RecipeManager.Application.Common.Results;
using RecipeManager.Application.Contracts.Users;
using RecipeManager.Application.Services;
using RecipeManager.Tests.Fixtures;

namespace RecipeManager.Tests.Services;

public class UserServiceTests : IDisposable
{
    private readonly Infrastructure.Persistence.ApplicationDbContext _db;
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _db = TestDbContextFactory.Create();
        var mapper = TestMapperFactory.Create();
        _sut = new UserService(mapper, _db);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task GetUserById_ExistingUser_ReturnsOk()
    {
        var user = await TestDataSeeder.SeedUserAsync(_db);

        var result = await _sut.GetUserByIdAsync(user.UserId);

        Assert.Equal(ResultStatus.Ok, result.Status);
        Assert.Equal(user.Email, result.Value!.Email);
        Assert.Contains(result.Value.Roles, r => r.Name == "User");
    }

    [Fact]
    public async Task UpdateUser_OwnProfile_ReturnsOk()
    {
        var user = await TestDataSeeder.SeedUserAsync(_db);

        var request = new UpdateUserRequest(
            FirstName: "Updated", LastName: null, Email: null, Phone: null);

        var result = await _sut.UpdateUserAsync(user.UserId, request);

        Assert.Equal(ResultStatus.Ok, result.Status);
        Assert.Equal("Updated", result.Value!.FirstName);
    }

    [Fact]
    public async Task UpdateUser_DuplicateEmail_ReturnsValidationError()
    {
        var user1 = await TestDataSeeder.SeedUserAsync(_db, "one@example.com");
        var user2 = await TestDataSeeder.SeedUserAsync(_db, "two@example.com");

        var request = new UpdateUserRequest(
            FirstName: null, LastName: null, Email: "one@example.com", Phone: null);

        var result = await _sut.UpdateUserAsync(user2.UserId, request);

        Assert.Equal(ResultStatus.ValidationError, result.Status);
        Assert.Contains("already exists", result.ErrorMessage);
    }

    [Fact]
    public async Task AssignRole_LastAdminToRemoveAdmin_ReturnsValidationError()
    {
        var admin = await TestDataSeeder.SeedUserAsync(_db, "admin@example.com", roleId: 1);
        var userRole = await TestDataSeeder.SeedUserAsync(_db, "user@example.com", roleId: 2);

        var result = await _sut.AssignRoleAsync(admin.UserId, userRole.UserRoles.First().RoleId);

        Assert.Equal(ResultStatus.ValidationError, result.Status);
        Assert.Contains("last administrator", result.ErrorMessage);
    }
}
