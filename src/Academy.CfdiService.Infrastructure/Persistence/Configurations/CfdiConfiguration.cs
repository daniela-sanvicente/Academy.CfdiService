using Academy.CfdiService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academy.CfdiService.Infrastructure.Persistence.Configurations;

public class CfdiConfiguration : IEntityTypeConfiguration<Cfdi>
{
    public void Configure(EntityTypeBuilder<Cfdi> builder)
    {
        builder.ToTable("Cfdi");

        builder.HasKey(cfdi => cfdi.Id);

        builder.Property(cfdi => cfdi.Id)
            .ValueGeneratedOnAdd();

        builder.Property(cfdi => cfdi.IssuerRfc)
            .IsRequired()
            .HasMaxLength(13);

        builder.Property(cfdi => cfdi.ReceiverRfc)
            .IsRequired()
            .HasMaxLength(13);

        builder.Property(cfdi => cfdi.Uuid)
            .IsRequired();

        builder.HasIndex(cfdi => cfdi.Uuid)
            .HasDatabaseName("IX_Cfdi_Uuid")
            .IsUnique();

        builder.Property(cfdi => cfdi.IssueDate)
            .IsRequired();

        builder.HasIndex(cfdi => new { cfdi.IssuerRfc, cfdi.ReceiverRfc })
            .HasDatabaseName("IX_Cfdi_Issuer_Receiver");

        builder.Property(cfdi => cfdi.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(cfdi => cfdi.LastUpdated)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        builder.HasIndex(cfdi => cfdi.IssueDate)
            .HasDatabaseName("IX_Cfdi_IssueDate");
    }
}
