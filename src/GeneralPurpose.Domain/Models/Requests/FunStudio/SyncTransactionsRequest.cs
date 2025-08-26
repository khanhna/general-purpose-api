using GeneralPurpose.Domain.Entities;

namespace GeneralPurpose.Domain.Models.Requests.FunStudio;

public class SyncTransactionsRequest
{
    public string Sig { get; set; } = string.Empty;
    public SyncTransactionsRequestDetail[] Transactions { get; set; } = [];
    public DateTime CurrentTime { get; set; }
}

public class SyncTransactionsRequestDetail
{
    public DateOnly RecordAt { get; set; }
    public TransactionProcessType Type { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
}