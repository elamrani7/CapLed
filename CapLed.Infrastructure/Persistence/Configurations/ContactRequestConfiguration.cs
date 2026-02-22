using StockManager.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StockManager.Infrastructure.Persistence.Configurations;

public class ContactRequestConfiguration : IEntityTypeConfiguration<ContactRequest>
{
    public void Configure(EntityTypeBuilder<ContactRequest> builder)
    {
        builder.HasKey(cr => cr.Id);

        builder.Property(cr => cr.SenderName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(cr => cr.SenderEmail)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(cr => cr.Status)
            .HasConversion<string>();

        builder.HasOne(cr => cr.Equipment)
            .WithMany(e => e.ContactRequests)
            .HasForeignKey(cr => cr.EquipmentId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

