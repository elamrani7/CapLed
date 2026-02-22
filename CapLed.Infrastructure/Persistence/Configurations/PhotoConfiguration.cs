using StockManager.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StockManager.Infrastructure.Persistence.Configurations;

public class PhotoConfiguration : IEntityTypeConfiguration<Photo>
{
    public void Configure(EntityTypeBuilder<Photo> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Url)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasOne(p => p.Equipment)
            .WithMany(e => e.Photos)
            .HasForeignKey(p => p.EquipmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

