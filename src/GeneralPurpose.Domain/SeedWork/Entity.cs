using Mediator;

namespace GeneralPurpose.Domain.SeedWork;

public abstract class BaseEntity
{
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedTime { get; private set; }
    public Guid LastUpdatedBy { get; private set; }
    public DateTime LastUpdatedTime { get; private set; }

    private List<INotification>? _domainEvents;
    public IReadOnlyCollection<INotification>? DomainEvents => _domainEvents?.AsReadOnly();

    public void AddDomainEvent(INotification eventItem)
    {
        _domainEvents ??= [];
        _domainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(INotification eventItem) => _domainEvents?.Remove(eventItem);

    public void ClearDomainEvents() => _domainEvents?.Clear();

    public void SetCreatedTime(Guid userIdentity)
    {
        CreatedTime = DateTime.UtcNow;
        LastUpdatedTime = DateTime.UtcNow;
        CreatedBy = userIdentity;
        LastUpdatedBy = userIdentity;
    }

    public void SetLastUpdatedTime(Guid userIdentity)
    {
        LastUpdatedTime = DateTime.UtcNow;
        LastUpdatedBy = userIdentity;
    }

    public void SetLastUpdatedTime() => LastUpdatedTime = DateTime.UtcNow;
}

public abstract class Entity<TKey> : BaseEntity where TKey : IEquatable<TKey>
{
    private int? _requestedHashCode;

    public virtual TKey Id { get; set; }

    public bool IsTransient() => EqualityComparer<TKey>.Default.Equals(Id, default);

    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is Entity<TKey>))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (GetType() != obj.GetType())
        {
            return false;
        }

        var item = (Entity<TKey>)obj;

        if (item.IsTransient() || IsTransient())
        {
            return false;
        }
        else
        {
            return EqualityComparer<TKey>.Default.Equals(item.Id, Id);
        }
    }

    public override int GetHashCode()
    {
        if (IsTransient()) return base.GetHashCode();
        
        _requestedHashCode ??= Id.GetHashCode() ^ 31; // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)
        return _requestedHashCode.Value;
    }

    public static bool operator ==(Entity<TKey> left, Entity<TKey> right)
    {
        if (Equals(left, null))
        {
            return (Equals(right, null)) ? true : false;
        }
        else
        {
            return left.Equals(right);
        }
    }

    public static bool operator !=(Entity<TKey> left, Entity<TKey> right) => !(left == right);
}