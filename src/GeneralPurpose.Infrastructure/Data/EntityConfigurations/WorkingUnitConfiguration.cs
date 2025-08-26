using GeneralPurpose.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeneralPurpose.Infrastructure.Data.EntityConfigurations;

public class WorkingUnitConfiguration : IEntityTypeConfiguration<WorkingUnit>
{
    public void Configure(EntityTypeBuilder<WorkingUnit> builder)
    {
        builder.ToTable("WorkingUnits");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name).HasMaxLength(256).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1024);
        builder.Property(x => x.Identifier).HasMaxLength(32).HasColumnType("char(32)");
        builder.Property(x => x.Note).HasMaxLength(1024);
        builder.Property(x => x.CreatedTime).HasPrecision(0);
        builder.Property(x => x.LastUpdatedTime).HasPrecision(0);

        builder.HasIndex(x => x.Identifier);
        
        builder.Ignore(x => x.CreatedBy);
        builder.Ignore(x => x.LastUpdatedBy);
    }
}