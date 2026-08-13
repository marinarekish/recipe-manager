using RecipeManager.Application.Contracts.Users;

namespace RecipeManager.Application.Interfaces;

public interface IUserService
{
    Task<List<UserResponse>> GetAllUsersAsync(CancellationToken ct = default);

    Task<UserResponse?> GetUserByIdAsync(int id, CancellationToken ct = default);

    Task<UserResponse> CreateUserAsync(CreateUserRequest user, CancellationToken ct = default);

    Task<UserUpdateResult> UpdateUserAsync(int id, UpdateUserRequest user, CancellationToken ct = default);

    Task<UserOperationStatus> DeleteUserAsync(int id, CancellationToken ct = default);

    Task<UserUpdateResult> AssignRoleAsync(int userId, int roleId, CancellationToken ct = default);
}
