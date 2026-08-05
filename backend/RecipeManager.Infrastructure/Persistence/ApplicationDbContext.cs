using RecipeManager.Domain.Entities;
using RecipeManager.Infrastructure.Configurations;

namespace RecipeManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Cuisine> Cuisines { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<Role> Roles { get; set; }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
    public DbSet<UserFavorite> UserFavorites { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}