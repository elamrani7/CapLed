using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManager.Core.Domain.Entities.Catalogue;

namespace StockManager.Infrastructure.Persistence.Configurations.Catalogue;

public class ArticleEtatDetailConfiguration : IEntityTypeConfiguration<ArticleEtatDetail>
{
    public void Configure(EntityTypeBuilder<ArticleEtatDetail> builder)
    {
        builder.ToTable("ARTICLE_ETAT_DETAIL");
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.ArticleId).IsUnique();

        builder.Property(e => e.GradeVisuel).HasMaxLength(5);
        builder.Property(e => e.PannesObservees).HasColumnType("TEXT");
        builder.Property(e => e.TestsFonctionnels).HasColumnType("TEXT");
        builder.Property(e => e.RevisionsEffectuees).HasColumnType("TEXT");

        builder.HasOne(e => e.Article)
            .WithOne(a => a.EtatDetail)
            .HasForeignKey<ArticleEtatDetail>(e => e.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
