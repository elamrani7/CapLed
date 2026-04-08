using StockManager.Core.Domain.Entities.Stock;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StockManager.Infrastructure.Persistence.Configurations.Stock;

public class StockQuantiteConfiguration : IEntityTypeConfiguration<StockQuantite>
{
    public void Configure(EntityTypeBuilder<StockQuantite> builder)
    {
        builder.ToTable("StockQuantites");
        builder.HasKey(sq => sq.Id);

        // UNIQUE constraint: one row per (Article × Depot)  MLD §2.2
        builder.HasIndex(sq => new { sq.ArticleId, sq.DepotId })
            .IsUnique()
            .HasDatabaseName("IX_StockQuantites_ArticleId_DepotId");

        builder.Property(sq => sq.Quantite)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(sq => sq.SeuilMinimum)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(sq => sq.LastUpdatedAt)
            .IsRequired();

        // FK → Equipments.Id  (ARTICLE in the MLD)
        builder.HasOne(sq => sq.Article)
            .WithMany(e => e.StockQuantites)
            .HasForeignKey(sq => sq.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        // FK → Depots.Id
        builder.HasOne(sq => sq.Depot)
            .WithMany(d => d.StockQuantites)
            .HasForeignKey(sq => sq.DepotId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
