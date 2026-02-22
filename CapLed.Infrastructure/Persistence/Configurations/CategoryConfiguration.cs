using StockManager.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StockManager.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Label)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(c => c.Label)
            .IsUnique();

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        // Relationships
        builder.HasMany(c => c.Equipments)
            .WithOne(e => e.Category)
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

