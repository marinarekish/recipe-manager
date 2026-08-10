using RecipeManager.Application.Contracts.Users;

namespace RecipeManager.Application.Interfaces;

public interface IUserService
{
    Task<List<UserResponse>> GetAllUsersAsync(CancellationToken ct = default);
    Task<UserResponse?> GetUserByIdAsync(int id, CancellationToken ct = default);
    Task<UserResponse> CreateUserAsync(CreateUserRequest? user, CancellationToken ct = default);
    Task<UserResponse?> UpdateUserAsync(int id, CreateUserRequest? user, CancellationToken ct = default);
    Task<bool> DeleteUserAsync(int id, CancellationToken ct = default);
}

// api/users/{me}/recipes
// api/users/