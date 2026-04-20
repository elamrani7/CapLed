using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManager.Core.Domain.Entities.Commercial;

namespace StockManager.Infrastructure.Persistence.Configurations.Commercial;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("CLIENT");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nom).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Prenom).HasMaxLength(100);
        builder.Property(c => c.Email).IsRequired().HasMaxLength(150);
        builder.Property(c => c.Telephone).HasMaxLength(20);
        builder.Property(c => c.Societe).HasMaxLength(200);
        builder.Property(c => c.Adresse).HasColumnType("TEXT");
        builder.Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        // Authentification site public
        builder.Property(c => c.PasswordHash).HasMaxLength(500);
        builder.Property(c => c.IsEmailConfirmed).HasDefaultValue(false);
        builder.Property(c => c.ConfirmationToken).HasMaxLength(200);
        builder.Property(c => c.TokenExpiry);

        builder.HasIndex(c => c.Email).IsUnique();
    }
}
