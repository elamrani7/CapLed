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

        // ── Step 1A: ERP / Site Vitrine columns ──────────────────────────────
        builder.Property(e => e.DatasheetUrl)
            .HasMaxLength(500);

        builder.Property(e => e.DisponibiliteSite)
            .IsRequired()
            .HasMaxLength(30)
            .HasDefaultValue("EN_STOCK");

        builder.Property(e => e.VisibleSite)
            .IsRequired()
            .HasDefaultValue(false);

        // ArticleSimilaireIds stored as TEXT (JSON array)
        builder.Property(e => e.ArticleSimilaireIds)
            .HasColumnType("TEXT");

        builder.Property(e => e.PrixVente)
            .HasPrecision(10, 2);

        builder.Property(e => e.PrixAchat)
            .HasPrecision(10, 2);
        // ─────────────────────────────────────────────────────────────────────

        // Relationships handled in other configurations or inferred
    }
}
