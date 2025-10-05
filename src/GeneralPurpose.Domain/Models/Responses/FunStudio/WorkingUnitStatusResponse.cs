namespace GeneralPurpose.Domain.Models.Responses.FunStudio;

public class WorkingUnitStatusResponse
{
    public bool IsActive { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Note { get; set; }
    public bool SkinRetouchEnabled { get; set; }
    public bool VintageProcessEnabled { get; set; }

    public ImageVintageConfig[] ImageVintageConfigs { get; set; } = [];
    public ImageCompositionConfigResponse[] ImageCompositionConfigs { get; set; } = [];
    
    public decimal SkinRetouchPrice { get; set; } = 0.006m;
    public decimal FaceSlimmingPrice { get; set; } = 0.003m;
    public decimal EyesEnlargerPrice { get; set; } = 0.001m;
    public decimal VintageProcessPrice { get; set; } = 0.0015m;
}

public class ImageCompositionConfigResponse
{
    public string FileName { get; set; } = string.Empty;
    public string BlendMode { get; set; } = string.Empty;
    public int Threshold { get; set; }
    public int Feather { get; set; }
    public decimal Opacity { get; set; }
    public bool InvertThreshold { get; set; }
    public DateTime LastUpdatedTime { get; set; }
}

public class ImageVintageConfig
{
    public string Code { get; set; } = string.Empty;
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
    public DateTime LastUpdatedTime { get; set; }
}