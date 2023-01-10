using DocumentsStorage.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocumentsStorage.Infrastructure.Data.Config;

public class DocumentDataConfiguration : IEntityTypeConfiguration<DataPayload>
{
    public void Configure(EntityTypeBuilder<DataPayload> builder)
    {
        builder.ToTable("Documents");
    }
}