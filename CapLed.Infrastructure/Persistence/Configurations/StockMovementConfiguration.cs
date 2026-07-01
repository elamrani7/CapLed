using StockManager.Core.Domain.Entities;
using StockManager.Core.Domain.Entities.Stock;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StockManager.Infrastructure.Persistence.Configurations;

public class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.HasKey(sm => sm.Id);

        builder.Property(sm => sm.Type)
            .HasConversion<string>();

        builder.Property(sm => sm.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.HasOne(sm => sm.Equipment)
            .WithMany(e => e.StockMovements)
            .HasForeignKey(sm => sm.EquipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sm => sm.User)
            .WithMany(u => u.PerformedMovements)
            .HasForeignKey(sm => sm.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(sm => sm.TypeMouvement)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("ENTREE");

        // ── Step 1B: FK relationships to Depots ───────────────────────────────
        builder.HasOne(sm => sm.DepotSource)
            .WithMany(d => d.MouvementsSource)
            .HasForeignKey(sm => sm.DepotSourceId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sm => sm.DepotDestination)
            .WithMany(d => d.MouvementsDestination)
            .HasForeignKey(sm => sm.DepotDestinationId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
        // ─────────────────────────────────────────────────────────────────────
    }
}
