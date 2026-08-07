using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeManager.Domain.Entities;

namespace RecipeManager.Infrastructure.Configurations;

public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.ToTable("ingredients");
        
        builder.HasKey(c => c.IngredientId);

        builder.Property(c => c.IngredientId)
            .HasColumnName("ingredient_id")
            .ValueGeneratedOnAdd();
        
        builder.Property(i => i.Name)
            .HasColumnName("name")
            .HasMaxLength(50)
            .IsRequired();
        
        // unique index
        builder.HasIndex(i => i.Name).IsUnique();
        
        // seeding 
        builder.HasData(
            new Ingredient { IngredientId = 1, Name = "Milk" },
            new Ingredient { IngredientId = 2, Name = "Sugar" },
            new Ingredient { IngredientId = 3, Name = "Potato" },
            new Ingredient { IngredientId = 4, Name = "Rice" },
            new Ingredient { IngredientId = 5, Name = "Sour cream" },
            new Ingredient { IngredientId = 6, Name = "Salt" },
            new Ingredient { IngredientId = 7, Name = "Egg" },
            new Ingredient { IngredientId = 8, Name = "Pork meat" },
            new Ingredient { IngredientId = 9, Name = "Cheese" },
            new Ingredient { IngredientId = 10, Name = "Chicken breast" },
            new Ingredient { IngredientId = 11, Name = "Tomato" },
            new Ingredient { IngredientId = 12, Name = "Carrot" },
            new Ingredient { IngredientId = 13, Name = "Olive Oil" },
            new Ingredient { IngredientId = 14, Name = "Pepperoni" }
            );
    }
}