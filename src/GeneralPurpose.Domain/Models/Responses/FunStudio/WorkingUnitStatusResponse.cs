namespace GeneralPurpose.Domain.Models.Responses.FunStudio;

public class WorkingUnitStatusResponse
{
    public bool IsActive { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Note { get; set; }
    public decimal SkinRetouchPrice { get; set; } = 0.006m;
    public decimal FaceSlimmingPrice { get; set; } = 0.003m;
    public decimal EyesEnlargerPrice { get; set; } = 0.001m;
}