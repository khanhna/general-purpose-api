using GeneralPurpose.Domain.SeedWork;

namespace GeneralPurpose.Domain.Entities;

public class ImageLutCubeSetting : Entity<int>, IAggregateRoot
{
    public int? AppSystemId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    
    public AppSystem? AppSystem { get; set; }
}