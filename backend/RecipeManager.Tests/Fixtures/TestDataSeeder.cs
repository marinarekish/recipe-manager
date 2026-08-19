using Microsoft.EntityFrameworkCore;
using RecipeManager.Domain.Entities;
using RecipeManager.Infrastructure.Persistence;

namespace RecipeManager.Tests.Fixtures;

public static class TestDataSeeder
{
    public static async Task SeedRolesAsync(ApplicationDbContext db)
    {
        if (await db.Roles.CountAsync() > 0) return;

        db.Roles.AddRange(
            new Role { RoleId = 1, Name = "Administrator" },
            new Role { RoleId = 2, Name = "User" }
        );
        await db.SaveChangesAsync();
    }

    public static async Task<User> SeedUserAsync(
        ApplicationDbContext db,
        string email = "test@example.com",
        string firstName = "Test",
        string lastName = "User",
        int roleId = 2)
    {
        await SeedRolesAsync(db);

        var user = new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            UserRoles = [new UserRole { RoleId = roleId }]
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();
        return user;
    }

    public static async Task<(User author, Category cat, Cuisine cuisine, Ingredient ing, Recipe recipe)>
        SeedRecipeAsync(ApplicationDbContext db, string? authorEmail = null)
    {
        await SeedRolesAsync(db);

        var author = new User
        {
            FirstName = "Author",
            LastName = "User",
            Email = authorEmail ?? "author@example.com",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            UserRoles = [new UserRole { RoleId = 2 }]
        };
        db.Users.Add(author);

        var cat = new Category { Name = "dinner" };
        var cuisine = new Cuisine { Name = "italian" };
        var ing = new Ingredient { Name = "garlic" };
        db.Categories.Add(cat);
        db.Cuisines.Add(cuisine);
        db.Ingredients.Add(ing);
        await db.SaveChangesAsync();

        var recipe = new Recipe
        {
            AuthorId = author.UserId,
            CuisineId = cuisine.CuisineId,
            CategoryId = cat.CategoryId,
            Title = "Pasta",
            PrepTimeMinutes = 10,
            CookTimeMinutes = 20,
            Servings = 4,
            Instructions = "Cook pasta.",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            RecipeIngredients =
            [
                new RecipeIngredient
                {
                    IngredientId = ing.IngredientId,
                    Amount = 2,
                    Unit = "cloves"
                }
            ]
        };
        db.Recipes.Add(recipe);
        await db.SaveChangesAsync();

        return (author, cat, cuisine, ing, recipe);
    }
}
