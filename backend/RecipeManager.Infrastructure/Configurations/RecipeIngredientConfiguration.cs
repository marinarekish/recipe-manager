using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeManager.Domain.Entities;

namespace RecipeManager.Infrastructure.Configurations;

public class RecipeIngredientConfiguration : IEntityTypeConfiguration<RecipeIngredient>
{
    public void Configure(EntityTypeBuilder<RecipeIngredient> builder)
    {
        builder.ToTable("recipe_ingredients", t =>
        {
            t.HasCheckConstraint("CK_recipe_ingredients_amount", "amount > 0");
        });

        builder.HasKey(ri => new { ri.RecipeId, ri.IngredientId });

        builder.Property(ri => ri.RecipeId)
            .HasColumnName("recipe_id")
            .IsRequired();

        builder.Property(ri => ri.IngredientId)
            .HasColumnName("ingredient_id")
            .IsRequired();

        builder.Property(ri => ri.Amount)
            .HasColumnName("amount")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(ri => ri.Unit)
            .HasColumnName("unit")
            .HasMaxLength(10)
            .IsRequired();

        builder.HasOne(ri => ri.Recipe)
            .WithMany(r => r.RecipeIngredients)
            .HasForeignKey(ri => ri.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ri => ri.Ingredient)
            .WithMany(i => i.RecipeIngredients)
            .HasForeignKey(ri => ri.IngredientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}