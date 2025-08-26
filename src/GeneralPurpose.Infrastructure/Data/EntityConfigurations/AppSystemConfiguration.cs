using GeneralPurpose.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeneralPurpose.Infrastructure.Data.EntityConfigurations;

public class AppSystemConfiguration : IEntityTypeConfiguration<AppSystem>
{
    public void Configure(EntityTypeBuilder<AppSystem> builder)
    {
        builder.ToTable("AppSystems");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name).HasMaxLength(256).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1024);
        builder.Property(x => x.CreatedTime).HasPrecision(0);
        builder.Property(x => x.LastUpdatedTime).HasPrecision(0);
        
        builder.Ignore(x => x.CreatedBy);
        builder.Ignore(x => x.LastUpdatedBy);
    }
}