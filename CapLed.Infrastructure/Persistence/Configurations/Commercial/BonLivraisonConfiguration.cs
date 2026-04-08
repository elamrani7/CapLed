using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManager.Core.Domain.Entities.Commercial;

namespace StockManager.Infrastructure.Persistence.Configurations.Commercial;

public class BonLivraisonConfiguration : IEntityTypeConfiguration<BonLivraison>
{
    public void Configure(EntityTypeBuilder<BonLivraison> builder)
    {
        builder.ToTable("BON_LIVRAISON");
        builder.HasKey(bl => bl.Id);

        builder.Property(bl => bl.NumeroBL).IsRequired().HasMaxLength(30);
        builder.Property(bl => bl.Statut).IsRequired().HasMaxLength(20).HasDefaultValue("BROUILLON");
        builder.Property(bl => bl.DateLivraison).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
        builder.Property(bl => bl.AdresseLivraison).HasColumnType("TEXT");
        builder.Property(bl => bl.Transporteur).HasMaxLength(100);
        builder.Property(bl => bl.NumeroSuivi).HasMaxLength(100);

        builder.HasIndex(bl => bl.NumeroBL).IsUnique();

        // FK: Client
        builder.HasOne(bl => bl.Client)
            .WithMany()
            .HasForeignKey(bl => bl.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        // FK: BonCommande (Optional)
        builder.HasOne(bl => bl.BonCommande)
            .WithMany(bc => bc.BonsLivraison)
            .HasForeignKey(bl => bl.BonCommandeId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class LigneBLConfiguration : IEntityTypeConfiguration<LigneBL>
{
    public void Configure(EntityTypeBuilder<LigneBL> builder)
    {
        builder.ToTable("LIGNE_BL");
        builder.HasKey(l => l.Id);

        builder.HasOne(l => l.BonLivraison)
            .WithMany(bl => bl.Lignes)
            .HasForeignKey(l => l.BonLivraisonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(l => l.Article)
            .WithMany()
            .HasForeignKey(l => l.ArticleId)
            .OnDelete(DeleteBehavior.Restrict);
            
        // Lot/SN references (manual link, no explicit FK here to simplify if needed, 
        // but it's better to have them for data integrity if they exist in DB)
    }
}
