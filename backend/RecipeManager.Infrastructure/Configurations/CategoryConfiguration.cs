using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeManager.Domain.Entities;

namespace RecipeManager.Infrastructure.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");
        
        builder.HasKey(c => c.CategoryId);

        builder.Property(c => c.CategoryId)
            .HasColumnName("category_id")
            .ValueGeneratedOnAdd();
        
        builder.Property(c => c.Name)
            .HasColumnName("name")
            .HasMaxLength(50)
            .IsRequired();
        
        // unique index
        builder.HasIndex(c => c.Name).IsUnique();
        
        // seeding
        builder.HasData(
            new Category{CategoryId = 1, Name = "Breakfast"},
            new Category{CategoryId = 2, Name = "Lunch"},
            new Category{CategoryId = 3, Name = "Dinner"},
            new Category{CategoryId = 4, Name = "Dessert"},
            new Category{CategoryId = 5, Name = "Salad"},
            new Category{CategoryId = 6, Name = "Drinks"},
            new Category{CategoryId = 7, Name = "Soup"},
            new Category{CategoryId = 8, Name = "Main Course"}
        );
    }
}