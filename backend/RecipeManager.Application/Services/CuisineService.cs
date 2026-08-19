using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Application.Common.Results;
using RecipeManager.Application.Contracts.Cuisines;
using RecipeManager.Application.Interfaces;
using RecipeManager.Domain.Entities;
using RecipeManager.Infrastructure.Persistence;

namespace RecipeManager.Application.Services;

public class CuisineService(
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

    public async Task<Result<CuisineResponse>> GetCuisineByIdAsync(int id, CancellationToken ct = default)
    {
        var cuisine = await context.Cuisines
            .AsNoTracking()
            .Where(c => c.CuisineId == id)
            .ProjectTo<CuisineResponse>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);

        return cuisine is not null
            ? Result<CuisineResponse>.Ok(cuisine)
            : Result<CuisineResponse>.NotFound();
    }

    public async Task<Result<CuisineResponse>> GetOrCreateAsync(string name, CancellationToken ct = default)
    {
        var trimmedName = name.Trim();

        if (string.IsNullOrWhiteSpace(trimmedName))
            return Result<CuisineResponse>.ValidationError("Cuisine name cannot be empty.");

        var storeName = trimmedName.ToLowerInvariant();

        var cuisine = await context.Cuisines
            .FirstOrDefaultAsync(
                c => c.Name.ToLower() == storeName.ToLower(),
                ct);

        if (cuisine is not null)
            return Result<CuisineResponse>.Ok(mapper.Map<CuisineResponse>(cuisine));

        cuisine = new Cuisine { Name = storeName };
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

        return Result<CuisineResponse>.Ok(mapper.Map<CuisineResponse>(cuisine));
    }

    public async Task<Result> DeleteCuisineAsync(int id, CancellationToken ct = default)
    {
        var cuisineToDelete = await context.Cuisines
            .FirstOrDefaultAsync(c => c.CuisineId == id, ct);

        if (cuisineToDelete is null)
            return Result.NotFound();

        var inUse = await context.Recipes
            .AsNoTracking()
            .AnyAsync(r => r.CuisineId == id, ct);

        if (inUse)
            return Result.Conflict("Cuisine is used by one or more recipes.");

        context.Cuisines.Remove(cuisineToDelete);

        try
        {
            await context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            return Result.Conflict("Cuisine is used by one or more recipes.");
        }

        return Result.NoContent();
    }
}