using GeneralPurpose.Domain.SeedWork;

namespace GeneralPurpose.Domain.Entities;

public class AppSystem : Entity<int>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Note { get; set; }

    public List<WorkingUnit>? WorkingUnits { get; set; }
    public List<ImageCompositionConfig>? ImageCompositionConfigs { get; set; }
}