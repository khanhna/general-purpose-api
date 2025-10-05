using GeneralPurpose.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeneralPurpose.Infrastructure.Data.EntityConfigurations;

public class ImageCompositionConfiguration : IEntityTypeConfiguration<ImageCompositionConfig>
{
    public void Configure(EntityTypeBuilder<ImageCompositionConfig> builder)
    {
        builder.ToTable("ImageCompositionConfigs");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.FileName).HasMaxLength(32).IsRequired();
        builder.Property(x => x.BlendMode).HasMaxLength(16).IsRequired();
        builder.Property(x => x.LastUpdatedTime).HasPrecision(0);
        
        builder.Ignore(x => x.CreatedBy);
        builder.Ignore(x => x.CreatedTime);
        builder.Ignore(x => x.LastUpdatedBy);
    }
}