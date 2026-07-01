using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManager.Core.Domain.Entities.Catalogue;

namespace StockManager.Infrastructure.Persistence.Configurations.Catalogue;

public class ChampSpecifiqueConfiguration : IEntityTypeConfiguration<ChampSpecifique>
{
    public void Configure(EntityTypeBuilder<ChampSpecifique> builder)
    {
        builder.ToTable("CHAMP_SPECIFIQUE");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.NomChamp).IsRequired().HasMaxLength(100);
        builder.Property(c => c.TypeDonnee).IsRequired().HasMaxLength(20).HasDefaultValue("TEXTE");
        builder.Property(c => c.Ordre).HasDefaultValue(0);

        builder.HasOne(c => c.Categorie)
            .WithMany(cat => cat.ChampsSpecifiques)
            .HasForeignKey(c => c.CategorieId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class ArticleChampValeurConfiguration : IEntityTypeConfiguration<ArticleChampValeur>
{
    public void Configure(EntityTypeBuilder<ArticleChampValeur> builder)
    {
        builder.ToTable("ARTICLE_CHAMP_VALEUR");
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Valeur).HasColumnType("TEXT");

        builder.HasIndex(v => new { v.ArticleId, v.ChampSpecifiqueId }).IsUnique();

        builder.HasOne(v => v.Article)
            .WithMany(a => a.ChampsSpecifiques)
            .HasForeignKey(v => v.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(v => v.ChampSpecifique)
            .WithMany(c => c.ArticleValues)
            .HasForeignKey(v => v.ChampSpecifiqueId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
