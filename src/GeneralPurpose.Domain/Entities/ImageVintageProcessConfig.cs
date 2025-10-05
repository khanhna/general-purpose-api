using GeneralPurpose.Domain.SeedWork;

namespace GeneralPurpose.Domain.Entities;

public class ImageVintageProcessConfig : Entity<int>, IAggregateRoot
{
    public int? WorkingUnitId { get; set; }
    public ImageVintageProcessConfigProfile Code { get; set; }
    public decimal Contrast { get; set; }
    public decimal Grain { get; set; }
    public decimal Vignette { get; set; }
    public decimal Fade { get; set; }
    public decimal TintIntensity { get; set; }
    public decimal Dust { get; set; }
    public int Scratches { get; set; }
    public int Hairs { get; set; }
    public decimal Blur { get; set; }
    public int RedAdjustment { get; set; }
    public int GreenAdjustment { get; set; }
    public int BlueAdjustment { get; set; }
    public decimal Brightness { get; set; }
    public bool IsActive { get; set; }

    public WorkingUnit? WorkingUnit { get; set; }
}

public enum ImageVintageProcessConfigProfile
{
    Default
}