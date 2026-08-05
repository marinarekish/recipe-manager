using RecipeManager.Domain.Entities;

namespace RecipeManager.Application.Interfaces;

public interface IUserService
{
    Task<List<User>> GetAllAsync(CancellationToken ct = default);
    Task<User?> GetByIdAsync(int id, CancellationToken ct = default);
    
    Task<User> CreateAsync(User? user, CancellationToken ct = default);
}