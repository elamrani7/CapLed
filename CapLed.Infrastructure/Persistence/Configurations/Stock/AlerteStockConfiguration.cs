using StockManager.Core.Domain.Entities.Stock;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StockManager.Infrastructure.Persistence.Configurations.Stock;

public class AlerteStockConfiguration : IEntityTypeConfiguration<AlerteStock>
{
    public void Configure(EntityTypeBuilder<AlerteStock> builder)
    {
        builder.ToTable("AlertesStock");
        builder.HasKey(a => a.Id);

        // Composite INDEX for efficient per-article/depot alert lookups
        builder.HasIndex(a => new { a.ArticleId, a.DepotId })
            .HasDatabaseName("IX_AlertesStock_ArticleId_DepotId");

        builder.Property(a => a.NiveauAlerte)
            .IsRequired()
            .HasMaxLength(20);  // AVERTISSEMENT | CRITIQUE | RUPTURE

        builder.Property(a => a.QuantiteAuDeclenchement)
            .IsRequired();

        builder.Property(a => a.SeuilUtilise)
            .IsRequired();

        builder.Property(a => a.EstResolue)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(a => a.DateCreation)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.Property(a => a.DateResolution)
            .IsRequired(false);

        // FK → Equipments.Id
        builder.HasOne(a => a.Article)
            .WithMany(e => e.AlertesStock)
            .HasForeignKey(a => a.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        // FK → Depots.Id
        builder.HasOne(a => a.Depot)
            .WithMany(d => d.AlertesStock)
            .HasForeignKey(a => a.DepotId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
