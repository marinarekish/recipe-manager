using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeManager.Domain.Entities;

namespace RecipeManager.Infrastructure.Configurations;

public class CuisineConfiguration : IEntityTypeConfiguration<Cuisine>
{
    public void Configure(EntityTypeBuilder<Cuisine> builder)
    {
        builder.ToTable("cuisines");
        
        builder.HasKey(c => c.CuisineId);

        builder.Property(c => c.CuisineId)
            .HasColumnName("cuisine_id")
            .ValueGeneratedOnAdd();
        
        builder.Property(c => c.Name)
            .HasColumnName("name")
            .HasMaxLength(50)
            .IsRequired();

        // unique index
        builder.HasIndex(c => c.Name).IsUnique();
        
        // seeding
        builder.HasData(
            new Cuisine { CuisineId = 1, Name = "Italian" },
            new Cuisine { CuisineId = 2, Name = "English" },
            new Cuisine { CuisineId = 3, Name = "French" },
            new Cuisine { CuisineId = 4, Name = "Mediterranean" },
            new Cuisine { CuisineId = 5, Name = "Spanish" },
            new Cuisine { CuisineId = 6, Name = "Slavic" }
        );
    }  
}