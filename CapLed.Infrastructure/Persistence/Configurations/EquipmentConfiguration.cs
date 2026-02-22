using StockManager.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StockManager.Infrastructure.Persistence.Configurations;

public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Reference)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(e => e.Reference)
            .IsUnique();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(e => e.Condition)
            .HasConversion<string>();

        builder.Property(e => e.Quantity)
            .HasDefaultValue(0);

        builder.Property(e => e.MinThreshold)
            .HasDefaultValue(5);

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        // Relationships handled in other configurations or inferred
    }
}

