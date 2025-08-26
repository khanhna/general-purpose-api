using GeneralPurpose.Domain.SeedWork;

namespace GeneralPurpose.Domain.Entities;

public class Transaction : Entity<int>, IAggregateRoot
{
    public int? WorkingUnitId { get; set; }
    public DateTime RecordAt { get; set; }
    public TransactionProcessType Type { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }

    public WorkingUnit? WorkingUnit { get; set; }
}

public enum TransactionProcessType
{
    SkinSmooth = 1,
    FaceSlim = 2,
    EyesEnlarge = 3,
}