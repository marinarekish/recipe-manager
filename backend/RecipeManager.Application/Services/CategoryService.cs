using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Application.Common.Results;
using RecipeManager.Application.Contracts.Categories;
using RecipeManager.Application.Interfaces;
using RecipeManager.Domain.Entities;
using RecipeManager.Infrastructure.Persistence;

namespace RecipeManager.Application.Services;

public class CategoryService(
    IMapper mapper,
    ApplicationDbContext context) : ICategoryService
{
    public async Task<List<CategoryResponse>> GetAllCategoriesAsync(CancellationToken ct = default)
    {
        return await context.Categories
            .AsNoTracking()
            .ProjectTo<CategoryResponse>(mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }

    public async Task<Result<CategoryResponse>> GetCategoryByIdAsync(int id, CancellationToken ct = default)
    {
        var category = await context.Categories
            .AsNoTracking()
            .Where(c => c.CategoryId == id)
            .ProjectTo<CategoryResponse>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);

        return category is not null
            ? Result<CategoryResponse>.Ok(category)
            : Result<CategoryResponse>.NotFound();
    }

    public async Task<Result<CategoryResponse>> GetOrCreateAsync(string name, CancellationToken ct = default)
    {
        var trimmedName = name.Trim();

        if (string.IsNullOrWhiteSpace(trimmedName))
            return Result<CategoryResponse>.ValidationError("Category name cannot be empty.");

        var storeName = trimmedName.ToLowerInvariant();

        var category = await context.Categories
            .FirstOrDefaultAsync(
                c => c.Name.ToLower() == storeName.ToLower(),
                ct);

        if (category is not null)
            return Result<CategoryResponse>.Ok(mapper.Map<CategoryResponse>(category));

        category = new Category { Name = storeName };
        context.Categories.Add(category);

        try
        {
            await context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            category = await context.Categories
                .FirstAsync(c => c.Name.ToLower() == storeName.ToLower(), ct);
        }

        return Result<CategoryResponse>.Ok(mapper.Map<CategoryResponse>(category));
    }
}