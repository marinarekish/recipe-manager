namespace RecipeManager.Infrastructure.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeManager.Domain.Entities;

public class RecipeIngredientConfiguration : IEntityTypeConfiguration<RecipeIngredient>
{
    public void Configure(EntityTypeBuilder<RecipeIngredient> builder)
    {
        builder.ToTable("recipe_ingredients");
        
        builder.HasKey(r => new { r.RecipeId, r.IngredientId });
        
        builder.Property(ri => ri.Amount)
            .HasColumnName("amount")
            .HasColumnType("decimal(5,2)")
            .IsRequired();
        
        builder.Property(ri => ri.Unit)
            .HasColumnName("unit")
            .HasMaxLength(10)
            .IsRequired();
        
        builder.Property(ri => ri.IngredientId)
            .HasColumnName("ingredient_id")
            .IsRequired();
        
        builder.Property(ri => ri.RecipeId)
            .HasColumnName("recipe_id")
            .IsRequired();
        
        builder.HasOne(ri => ri.Ingredient)
            .WithMany(i => i.RecipeIngredients)
            .HasForeignKey(ri => ri.IngredientId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // unique index
        builder.ToTable(t =>
        {
            t.HasCheckConstraint(
                "check_amount",
                "amount > 0");
        });
    }
}