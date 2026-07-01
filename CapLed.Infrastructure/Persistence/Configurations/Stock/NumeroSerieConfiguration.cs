using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManager.Core.Domain.Entities.Stock;

namespace StockManager.Infrastructure.Persistence.Configurations.Stock;

public class NumeroSerieConfiguration : IEntityTypeConfiguration<NumeroSerie>
{
    public void Configure(EntityTypeBuilder<NumeroSerie> builder)
    {
        builder.ToTable("NUMERO_SERIE");

        builder.HasKey(ns => ns.Id);

        builder.Property(ns => ns.NumeroSerieLabel)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(ns => ns.Statut)
            .IsRequired()
            .HasMaxLength(20)
            .HasConversion<string>();

        builder.Property(ns => ns.DateEntree)
            .IsRequired()
            .HasColumnType("DATE");

        // Unique constraint on NumeroSerie
        builder.HasIndex(ns => ns.NumeroSerieLabel)
            .IsUnique();

        // Relationships
        builder.HasOne(ns => ns.Article)
            .WithMany(a => a.NumerosSerie)
            .HasForeignKey(ns => ns.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ns => ns.Depot)
            .WithMany(d => d.NumerosSerie)
            .HasForeignKey(ns => ns.DepotId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
