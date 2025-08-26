namespace GeneralPurpose.Domain.Models.Requests.FunStudio;

public class TestGenerateSignatureRequest
{
    public string Identifier { get; set; } = string.Empty;
    public DateTime CurrenTime { get; set; }
}

public class TestGenerateSignatureResponse : TestGenerateSignatureRequest
{
    public string Sig { get; set; } = string.Empty;
}