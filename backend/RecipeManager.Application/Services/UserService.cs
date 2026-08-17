using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Application.Common.Results;
using RecipeManager.Application.Contracts.Users;
using RecipeManager.Application.Interfaces;
using RecipeManager.Domain.Entities;
using RecipeManager.Infrastructure.Persistence;

namespace RecipeManager.Application.Services;

public class UserService(
    IMapper mapper,
    ApplicationDbContext context) : IUserService
{
    private const string AdministratorRoleName = "Administrator";

    public async Task<List<UserResponse>> GetAllUsersAsync(CancellationToken ct = default)
    {
        return await context.Users
            .AsNoTracking()
            .OrderBy(u => u.UserId)
            .ProjectTo<UserResponse>(mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    public async Task<Result<UserResponse>> GetUserByIdAsync(int id, CancellationToken ct = default)
    {
        var user = await context.Users
            .AsNoTracking()
            .Where(u => u.UserId == id)
            .ProjectTo<UserResponse>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);

        return user is null
            ? Result<UserResponse>.NotFound()
            : Result<UserResponse>.Ok(user);
    }

    public async Task<Result<UserResponse>> CreateUserAsync(
        CreateUserRequest user,
        CancellationToken ct = default)
    {
        try
        {
            var email = user.Email.Trim().ToLowerInvariant();

            var emailExists = await context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Email == email, ct);

            if (emailExists)
                return Result<UserResponse>.ValidationError(
                    "User with this email already exists.");

            var defaultRoleId = await GetRoleIdByNameAsync("User", ct);

            var userToAdd = new User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = email,
                Phone = user.Phone,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            userToAdd.UserRoles.Add(new UserRole { RoleId = defaultRoleId });

            context.Users.Add(userToAdd);
            await context.SaveChangesAsync(ct);

            var created = await context.Users
                .AsNoTracking()
                .Where(u => u.UserId == userToAdd.UserId)
                .ProjectTo<UserResponse>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(ct);

            return Result<UserResponse>.Ok(created!);
        }
        catch (ArgumentException ex)
        {
            return Result<UserResponse>.ValidationError(ex.Message);
        }
    }

    public async Task<Result<UserResponse>> UpdateUserAsync(
        int id,
        UpdateUserRequest user,
        CancellationToken ct = default)
    {
        var userToUpdate = await context.Users
            .FirstOrDefaultAsync(u => u.UserId == id, ct);

        if (userToUpdate is null)
            return Result<UserResponse>.NotFound();

        try
        {
            if (user.Email is not null)
            {
                var email = user.Email.Trim().ToLowerInvariant();

                var emailExists = await context.Users
                    .AsNoTracking()
                    .AnyAsync(u => u.Email == email && u.UserId != id, ct);

                if (emailExists)
                    return Result<UserResponse>.ValidationError(
                        "User with this email already exists.");

                userToUpdate.Email = email;
            }

            if (user.FirstName is not null)
                userToUpdate.FirstName = user.FirstName;

            if (user.LastName is not null)
                userToUpdate.LastName = user.LastName;

            if (user.Phone is not null)
                userToUpdate.Phone = user.Phone;

            userToUpdate.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);

            var updatedUser = await context.Users
                .AsNoTracking()
                .Where(u => u.UserId == id)
                .ProjectTo<UserResponse>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(ct);

            return Result<UserResponse>.Ok(updatedUser!);
        }
        catch (ArgumentException ex)
        {
            return Result<UserResponse>.ValidationError(ex.Message);
        }
    }

    public async Task<Result> DeleteUserAsync(
        int id,
        CancellationToken ct = default)
    {
        var userToDelete = await context.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.UserId == id, ct);

        if (userToDelete is null)
            return Result.NotFound();

        try
        {
            var adminRoleId = await GetRoleIdByNameAsync(AdministratorRoleName, ct);

            if (userToDelete.UserRoles.Any(ur => ur.RoleId == adminRoleId) &&
                await GetAdminCountAsync(adminRoleId, ct) == 1)
                return Result.ValidationError("Cannot delete the last administrator.");

            context.Users.Remove(userToDelete);
            await context.SaveChangesAsync(ct);

            return Result.Ok();
        }
        catch (ArgumentException ex)
        {
            return Result.ValidationError(ex.Message);
        }
    }

    public async Task<Result<UserResponse>> AssignRoleAsync(
        int userId,
        int roleId,
        CancellationToken ct = default)
    {
        var user = await context.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.UserId == userId, ct);

        if (user is null)
            return Result<UserResponse>.NotFound();

        var role = await context.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.RoleId == roleId, ct);

        if (role is null)
            return Result<UserResponse>.NotFound();

        try
        {
            var adminRoleId = await GetRoleIdByNameAsync(AdministratorRoleName, ct);
            var isCurrentlyAdmin = user.UserRoles.Any(ur => ur.RoleId == adminRoleId);

            if (isCurrentlyAdmin &&
                role.RoleId != adminRoleId &&
                await GetAdminCountAsync(adminRoleId, ct) == 1)
                return Result<UserResponse>.ValidationError(
                    "Cannot remove the role from the last administrator.");

            foreach (var userRole in user.UserRoles.Where(ur => ur.RoleId != roleId).ToList())
                user.UserRoles.Remove(userRole);

            if (user.UserRoles.All(ur => ur.RoleId != roleId))
                user.UserRoles.Add(new UserRole { UserId = userId, RoleId = roleId });

            await context.SaveChangesAsync(ct);

            var updatedUser = await context.Users
                .AsNoTracking()
                .Where(u => u.UserId == userId)
                .ProjectTo<UserResponse>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(ct);

            return Result<UserResponse>.Ok(updatedUser!);
        }
        catch (ArgumentException ex)
        {
            return Result<UserResponse>.ValidationError(ex.Message);
        }
    }

    private async Task<int> GetRoleIdByNameAsync(string roleName, CancellationToken ct)
    {
        var roleId = await context.Roles
            .AsNoTracking()
            .Where(r => r.Name == roleName)
            .Select(r => r.RoleId)
            .FirstOrDefaultAsync(ct);

        if (roleId == 0)
            throw new ArgumentException($"Role '{roleName}' was not found.");

        return roleId;
    }

    private async Task<int> GetAdminCountAsync(int adminRoleId, CancellationToken ct)
    {
        return await context.UserRoles
            .AsNoTracking()
            .CountAsync(ur => ur.RoleId == adminRoleId, ct);
    }
}
