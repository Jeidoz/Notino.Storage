using DocumentsStorage.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocumentsStorage.Infrastructure.Data.Config;

public sealed class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("Documents");
        builder.Property(doc => doc.DocumentId)
            .IsRequired();
        builder.HasOne(doc => doc.Data)
            .WithOne()
            .HasForeignKey<DataPayload>(data => data.Id);
        builder.Navigation(doc => doc.Data).IsRequired();
        builder.HasIndex(doc => doc.DocumentId).IsUnique();
    }
}