using GeneralPurpose.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeneralPurpose.Infrastructure.Data.EntityConfigurations;

public class ImageVintageProcessConfiguration : IEntityTypeConfiguration<ImageVintageProcessConfig>
{
    public void Configure(EntityTypeBuilder<ImageVintageProcessConfig> builder)
    {
        builder.ToTable("ImageVintageProcessConfigs");
        builder.HasKey(x => x.Id);
        
        builder.Ignore(x => x.CreatedBy);
        builder.Ignore(x => x.CreatedTime);
        builder.Ignore(x => x.LastUpdatedBy);
        builder.Ignore(x => x.LastUpdatedTime);
    }
}