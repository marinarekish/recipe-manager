using Microsoft.EntityFrameworkCore;
using RecipeManager.Application.Interfaces;
using RecipeManager.Domain.Entities;
using RecipeManager.Infrastructure.Persistence;

namespace RecipeManager.Application.Services;

public class UserService(ApplicationDbContext context) : IUserService
{
    private readonly ApplicationDbContext _context = context;
    public Task<List<User>> GetAllAsync(CancellationToken ct = default)
    {
        return _context.Users.ToListAsync(cancellationToken: ct);
    }

    public Task<User?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<User> CreateAsync(User? user, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}