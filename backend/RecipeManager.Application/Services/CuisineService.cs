using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Application.Contracts.Cuisines;
using RecipeManager.Application.Interfaces;
using RecipeManager.Domain.Entities;
using RecipeManager.Infrastructure.Persistence;

namespace RecipeManager.Application.Services;

public class CuisineService (
    IMapper mapper,
    ApplicationDbContext context) : ICuisineService
{
    public async Task<List<CuisineResponse>> GetAllCuisinesAsync(CancellationToken ct = default)
    {
        return await context.Cuisines
            .AsNoTracking()
            .ProjectTo<CuisineResponse>(mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    public async Task<CuisineResponse?> GetCuisineByIdAsync(int id, CancellationToken ct = default)
    {
        return await context.Cuisines
            .AsNoTracking()
            .Where(c => c.CuisineId == id)
            .ProjectTo<CuisineResponse>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<CuisineResponse?> GetOrCreateAsync(string name, CancellationToken ct = default)
    {
        var trimmedName = name.Trim();
        
        if (string.IsNullOrWhiteSpace(trimmedName))
            throw new ArgumentException("Cuisine name cannot be empty.", nameof(name));

        var cuisine = await context.Cuisines
            .FirstOrDefaultAsync(c => c.Name.ToLower() == trimmedName.ToLower(), cancellationToken: ct);

        if (cuisine != null)
            return mapper.Map<CuisineResponse>(cuisine);

        cuisine = new Cuisine
        {
            Name = trimmedName
        };
        
        context.Cuisines.Add(cuisine);

        try
        {
            await context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            cuisine = await context.Cuisines
                .FirstAsync(c => c.Name.ToLower() == trimmedName.ToLower(), ct);
        }

        return mapper.Map<CuisineResponse>(cuisine);
    }
}