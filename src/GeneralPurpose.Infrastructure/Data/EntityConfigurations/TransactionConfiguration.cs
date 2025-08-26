using GeneralPurpose.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeneralPurpose.Infrastructure.Data.EntityConfigurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.RecordAt).HasPrecision(0);
        builder.Property(x => x.CreatedTime).HasPrecision(0);
        builder.Property(x => x.LastUpdatedTime).HasPrecision(0);

        builder.HasIndex(x => x.RecordAt);
        
        builder.Ignore(x => x.CreatedBy);
        builder.Ignore(x => x.LastUpdatedBy);
    }
}