using GeneralPurpose.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeneralPurpose.Infrastructure.Data.EntityConfigurations;

public class LutCubeSettingConfiguration : IEntityTypeConfiguration<ImageLutCubeSetting>
{
    public void Configure(EntityTypeBuilder<ImageLutCubeSetting> builder)
    {
        builder.ToTable("ImageLutCubeSettings");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Code).HasMaxLength(16).IsRequired();
        builder.Property(x => x.FileName).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Url).HasMaxLength(1024).IsRequired();
        builder.Property(x => x.LastUpdatedTime).HasPrecision(0);
        
        builder.HasIndex(x  => x.Code);
        builder.HasIndex(x  => x.FileName).IsUnique();
        
        builder.Ignore(x => x.CreatedBy);
        builder.Ignore(x => x.CreatedTime);
        builder.Ignore(x => x.LastUpdatedBy);
    }
}