using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using RecipeManager.Application.Mapping;
using RecipeManager.Infrastructure.Persistence;

namespace RecipeManager.Tests.Fixtures;

public static class TestDbContextFactory
{
    public static ApplicationDbContext Create()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}

public static class TestMapperFactory
{
    private static readonly MapperConfiguration Configuration = new(cfg =>
    {
        cfg.AddProfile<RecipeMappingProfile>();
        cfg.AddProfile<UserMappingProfile>();
        cfg.AddProfile<FavoriteMappingProfile>();
        cfg.AddProfile<RoleMappingProfile>();
        cfg.AddProfile<CategoryMappingProfile>();
        cfg.AddProfile<CuisineMappingProfile>();
        cfg.AddProfile<IngredientMappingProfile>();
    }, NullLoggerFactory.Instance);

    public static IMapper Create() => Configuration.CreateMapper();
}
