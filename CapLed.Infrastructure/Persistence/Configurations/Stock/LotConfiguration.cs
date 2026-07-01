using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManager.Core.Domain.Entities.Stock;

namespace StockManager.Infrastructure.Persistence.Configurations.Stock;

public class LotConfiguration : IEntityTypeConfiguration<Lot>
{
    public void Configure(EntityTypeBuilder<Lot> builder)
    {
        builder.ToTable("LOT");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.NumeroLot)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(l => l.Quantite)
            .IsRequired();

        builder.Property(l => l.Fournisseur)
            .HasMaxLength(150);

        builder.Property(l => l.DateEntree)
            .IsRequired()
            .HasColumnType("DATE");

        builder.Property(l => l.Garantie)
            .HasMaxLength(50);

        builder.Property(l => l.Certificat)
            .HasMaxLength(255);

        // Unique constraint: (NumeroLot, ArticleId, DepotId)
        builder.HasIndex(l => new { l.NumeroLot, l.ArticleId, l.DepotId })
            .IsUnique();

        // Relationships
        builder.HasOne(l => l.Article)
            .WithMany(a => a.Lots)
            .HasForeignKey(l => l.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(l => l.Depot)
            .WithMany(d => d.Lots)
            .HasForeignKey(l => l.DepotId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
