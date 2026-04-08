using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManager.Core.Domain.Entities.Commercial;

namespace StockManager.Infrastructure.Persistence.Configurations.Commercial;

public class LigneLeadConfiguration : IEntityTypeConfiguration<LigneLead>
{
    public void Configure(EntityTypeBuilder<LigneLead> builder)
    {
        builder.ToTable("LIGNE_LEAD");
        builder.HasKey(l => l.Id);

        builder.Property(l => l.QuantiteDemandee).IsRequired();
        builder.Property(l => l.Commentaire).HasColumnType("TEXT");

        builder.HasOne(l => l.Lead)
            .WithMany(ld => ld.Lignes)
            .HasForeignKey(l => l.LeadId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(l => l.Article)
            .WithMany()
            .HasForeignKey(l => l.ArticleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
