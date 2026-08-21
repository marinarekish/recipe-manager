using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeManager.Domain.Entities;

namespace RecipeManager.Infrastructure.Configurations;

public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.ToTable("recipes", t =>
        {
            t.HasCheckConstraint("CK_recipes_servings", "servings > 0");
            t.HasCheckConstraint("CK_recipes_prep_time", "prep_time_minutes > 0");
            t.HasCheckConstraint("CK_recipes_cook_time", "cook_time_minutes > 0");
        });

        builder.HasKey(r => r.RecipeId);

        builder.Property(r => r.RecipeId)
            .HasColumnName("recipe_id")
            .ValueGeneratedOnAdd();

        builder.Property(r => r.AuthorId)
            .HasColumnName("author_id")
            .IsRequired();

        builder.Property(r => r.CuisineId)
            .HasColumnName("cuisine_id")
            .IsRequired();

        builder.Property(r => r.CategoryId)
            .HasColumnName("category_id")
            .IsRequired();

        builder.Property(r => r.Title)
            .HasColumnName("title")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(r => r.PrepTimeMinutes)
            .HasColumnName("prep_time_minutes")
            .IsRequired();

        builder.Property(r => r.CookTimeMinutes)
            .HasColumnName("cook_time_minutes")
            .IsRequired();

        builder.Property(r => r.Servings)
            .HasColumnName("servings")
            .IsRequired();

        builder.Property(r => r.Instructions)
            .HasColumnName("instructions")
            .HasColumnType("text")
            .IsRequired(false);

        builder.Property(r => r.ImageUrl)
            .HasColumnName("image_url")
            .HasMaxLength(2048)
            .IsRequired(false);

        builder.Property(r => r.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        builder.Property(r => r.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        builder.HasOne(r => r.Author)
            .WithMany(u => u.CreatedRecipes)
            .HasForeignKey(r => r.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Category)
            .WithMany(c => c.Recipes)
            .HasForeignKey(r => r.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Cuisine)
            .WithMany(c => c.Recipes)
            .HasForeignKey(r => r.CuisineId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(r => r.RecipeIngredients)
            .WithOne(ri => ri.Recipe)
            .HasForeignKey(ri => ri.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.UserFavorites)
            .WithOne(uf => uf.Recipe)
            .HasForeignKey(uf => uf.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}