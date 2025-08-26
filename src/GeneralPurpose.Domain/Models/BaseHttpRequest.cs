namespace GeneralPurpose.Domain.Models;

public interface IPagingIndicate
{
    public string[] OrderBy { get; set; }
    public int? PageNo { get; set; }
    public int? PageSize { get; set; }
}
    
public class BaseHttpRequest : IPagingIndicate
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string[] OrderBy { get; set; } = [];
    public int? PageNo { get; set; }
    public int? PageSize { get; set; }
}