using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeManager.Domain.Entities;

namespace RecipeManager.Infrastructure.Configurations;

public class UserFavoriteConfiguration : IEntityTypeConfiguration<UserFavorite>
{
    public void Configure(EntityTypeBuilder<UserFavorite> builder)
    {
        builder.ToTable("user_favorites");
        
        builder.HasKey(uf => new { uf.RecipeId, uf.UserId });
        
        builder.Property(uf => uf.RecipeId)
            .HasColumnName("recipe_id")
            .IsRequired();
        
        builder.Property(uf => uf.UserId)
            .HasColumnName("user_id")
            .IsRequired();
        
        // TODO
        // CreatedAt = DateTime.UtcNow
        // UpdatedAt = DateTime.UtcNow
        builder.Property(uf => uf.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();
        
        builder.Property(uf => uf.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();
        
        builder.HasOne(x => x.User)
            .WithMany(x => x.UserFavorites)
            .HasForeignKey(x => x.UserId);

        builder.HasOne(x => x.Recipe)
            .WithMany(x => x.UserFavorites)
            .HasForeignKey(x => x.RecipeId);
    }
}