using StockManager.Core.Domain.Entities.Stock;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StockManager.Infrastructure.Persistence.Configurations.Stock;

public class DepotConfiguration : IEntityTypeConfiguration<Depot>
{
    public void Configure(EntityTypeBuilder<Depot> builder)
    {
        builder.ToTable("Depots");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Nom)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(d => d.Nom)
            .IsUnique();

        builder.Property(d => d.Adresse)
            .HasColumnType("TEXT");

        builder.Property(d => d.EstActif)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(d => d.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        // Relationships to StockMovements (DepotSourceId / DepotDestinationId)
        // are configured in StockMovementConfiguration to avoid circular config.
    }
}
