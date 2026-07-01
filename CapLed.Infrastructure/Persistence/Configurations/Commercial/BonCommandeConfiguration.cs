using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManager.Core.Domain.Entities.Commercial;

namespace StockManager.Infrastructure.Persistence.Configurations.Commercial;

public class BonCommandeConfiguration : IEntityTypeConfiguration<BonCommande>
{
    public void Configure(EntityTypeBuilder<BonCommande> builder)
    {
        builder.ToTable("BON_COMMANDE");
        builder.HasKey(bc => bc.Id);

        builder.Property(bc => bc.NumeroBC).IsRequired().HasMaxLength(30);
        builder.Property(bc => bc.Statut).IsRequired().HasMaxLength(30).HasDefaultValue("EN_ATTENTE");
        builder.Property(bc => bc.DateCommande).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
        builder.Property(bc => bc.Commentaire).HasColumnType("TEXT");

        builder.HasIndex(bc => bc.NumeroBC).IsUnique();

        // FK: Client
        builder.HasOne(bc => bc.Client)
            .WithMany()
            .HasForeignKey(bc => bc.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class LigneBCConfiguration : IEntityTypeConfiguration<LigneBC>
{
    public void Configure(EntityTypeBuilder<LigneBC> builder)
    {
        builder.ToTable("LIGNE_BC");
        builder.HasKey(l => l.Id);

        builder.HasOne(l => l.BonCommande)
            .WithMany(bc => bc.Lignes)
            .HasForeignKey(l => l.BonCommandeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(l => l.Article)
            .WithMany()
            .HasForeignKey(l => l.ArticleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
