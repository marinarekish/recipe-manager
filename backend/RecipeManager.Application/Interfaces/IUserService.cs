using RecipeManager.Application.Common.Results;
using RecipeManager.Application.Contracts.Users;

namespace RecipeManager.Application.Interfaces;

public interface IUserService
{
    Task<List<UserResponse>> GetAllUsersAsync(CancellationToken ct = default);

    Task<Result<UserResponse>> GetUserByIdAsync(int id, CancellationToken ct = default);

    Task<Result<UserResponse>> CreateUserAsync(CreateUserRequest user, CancellationToken ct = default);

    Task<Result<UserResponse>> UpdateUserAsync(int id, UpdateUserRequest user, CancellationToken ct = default);

    Task<Result> DeleteUserAsync(int id, CancellationToken ct = default);

    Task<Result<UserResponse>> AssignRoleAsync(int userId, int roleId, CancellationToken ct = default);
}
