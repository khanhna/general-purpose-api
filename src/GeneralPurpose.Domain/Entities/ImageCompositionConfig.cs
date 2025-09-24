using GeneralPurpose.Domain.SeedWork;

namespace GeneralPurpose.Domain.Entities;

public class ImageCompositionConfig : Entity<int>, IAggregateRoot
{
    public int? AppSystemId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string BlendMode { get; set; } = string.Empty;
    public int Threshold { get; set; }
    public int Feather { get; set; }
    public decimal Opacity { get; set; }
    public bool InvertThreshold { get; set; }
    public bool IsActive { get; set; }

    public AppSystem? AppSystem { get; set; }
}