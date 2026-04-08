using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManager.Core.Domain.Entities.Commercial;

namespace StockManager.Infrastructure.Persistence.Configurations.Commercial;

public class LeadConfiguration : IEntityTypeConfiguration<Lead>
{
    public void Configure(EntityTypeBuilder<Lead> builder)
    {
        builder.ToTable("LEAD");
        builder.HasKey(l => l.Id);

        builder.Property(l => l.NumeroDevis).IsRequired().HasMaxLength(30);
        builder.Property(l => l.Statut).IsRequired().HasMaxLength(20).HasDefaultValue("NOUVEAU");
        builder.Property(l => l.DateSoumission).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
        builder.Property(l => l.Commentaire).HasColumnType("TEXT");
        builder.Property(l => l.SourceAcquisition).HasMaxLength(20).HasDefaultValue("DIRECT");

        builder.HasIndex(l => l.NumeroDevis).IsUnique();

        // FK: Client
        builder.HasOne(l => l.Client)
            .WithMany(c => c.Leads)
            .HasForeignKey(l => l.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        // FK: Commercial (User) — nullable
        builder.HasOne(l => l.Commercial)
            .WithMany()
            .HasForeignKey(l => l.CommercialId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
