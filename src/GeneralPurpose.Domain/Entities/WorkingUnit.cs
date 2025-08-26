using GeneralPurpose.Domain.SeedWork;

namespace GeneralPurpose.Domain.Entities;

public class WorkingUnit : Entity<int>, IAggregateRoot
{
    public int? SystemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Note { get; set; }
    public string? Identifier { get; set; }
    public bool IsActive { get; set; }
    public DateTime? ExpireAt { get; set; }

    public AppSystem? AppSystem { get; set; }

    public List<Transaction>? Transactions { get; set; }
}