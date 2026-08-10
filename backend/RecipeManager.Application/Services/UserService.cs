using Microsoft.EntityFrameworkCore;
using RecipeManager.Application.Contracts.Users;
using RecipeManager.Application.Interfaces;
using RecipeManager.Domain.Entities;
using RecipeManager.Infrastructure.Persistence;

namespace RecipeManager.Application.Services;

public class UserService(ApplicationDbContext context) : IUserService
{
    public Task<List<UserResponse>> GetAllUsersAsync(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<UserResponse?> GetUserByIdAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<UserResponse> CreateUserAsync(CreateUserRequest? user, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<UserResponse?> UpdateUserAsync(int id, CreateUserRequest? user, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteUserAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}