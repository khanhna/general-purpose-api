using GeneralPurpose.Domain.SeedWork;

namespace GeneralPurpose.Infrastructure.Persistence.KeyTypedRepositories;

public interface IGenericRepository_Create<in TEntity, in TKey>
    where TKey : IEquatable<TKey>
    where TEntity : Entity<TKey>, IAggregateRoot
{
    void Add(TEntity entity);
    void Add(IEnumerable<TEntity> entities);
}

public abstract partial class BaseGenericRepository<TEntity, TKey>
    where TKey : IEquatable<TKey>
    where TEntity : Entity<TKey>, IAggregateRoot
{
    public void Add(TEntity entity)
    {
        entity.SetCreatedTime(_identityService.GetUserIdentity());
        Context.Set<TEntity>().Add(entity);
    }

    public void Add(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            entity.SetCreatedTime(_identityService.GetUserIdentity());
            Context.Set<TEntity>().Add(entity);
        }
    }
}