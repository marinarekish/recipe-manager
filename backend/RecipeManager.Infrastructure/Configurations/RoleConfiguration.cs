using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeManager.Domain.Entities;

namespace RecipeManager.Infrastructure.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");
        
        builder.HasKey(r => r.RoleId);
        
        builder.Property(r => r.RoleId)
            .HasColumnName("role_id")
            .ValueGeneratedOnAdd();
        
        builder.Property(r => r.Name)
            .HasColumnName("name")
            .HasMaxLength(50)
            .IsRequired();
        
        // relationships
        builder.HasMany(r => r.UserRoles)
            .WithOne(ur => ur.Role)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // unique index
        builder.HasIndex(r => r.Name).IsUnique();
        
        // seeding
        builder.HasData(
            new Role { RoleId = 1, Name = "Administrator" },
            new Role { RoleId = 2, Name = "User" }
        );
    }
}