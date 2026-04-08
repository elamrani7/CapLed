using StockManager.Core.Domain.Entities.Catalogue;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StockManager.Infrastructure.Persistence.Configurations.Catalogue;

public class FamilleConfiguration : IEntityTypeConfiguration<Famille>
{
    public void Configure(EntityTypeBuilder<Famille> builder)
    {
        builder.ToTable("Familles");
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Code)
            .IsRequired()
            .HasMaxLength(30);

        builder.HasIndex(f => f.Code)
            .IsUnique();

        builder.Property(f => f.Libelle)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.Description)
            .HasColumnType("TEXT");

        builder.Property(f => f.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        // Relationship: Famille 1 → N Categories
        // (Categories.FamilleId FK is wired in CategoryConfiguration)
        builder.HasMany(f => f.Categories)
            .WithOne(c => c.Famille)
            .HasForeignKey(c => c.FamilleId)
            .OnDelete(DeleteBehavior.SetNull); // Null out FamilleId if family deleted
    }
}
