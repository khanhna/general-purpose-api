using GeneralPurpose.Domain.Entities;

namespace GeneralPurpose.Domain.Models.Responses.FunStudio;

public class SyncTransactionsResponse
{
    public SyncTransactionsResponseDetail[] Transactions { get; set; } = [];
}

public class SyncTransactionsResponseDetail
{
    public DateOnly RecordAt { get; set; }
    public TransactionProcessType Type { get; set; }
    public DateTime? LastSyncAt { get; set; }
}