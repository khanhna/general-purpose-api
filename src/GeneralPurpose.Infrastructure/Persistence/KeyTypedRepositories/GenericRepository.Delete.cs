using GeneralPurpose.Domain.SeedWork;

namespace GeneralPurpose.Infrastructure.Persistence.KeyTypedRepositories;

public interface IGenericRepository_Delete<in TEntity, in TKey>
    where TEntity : IAggregateRoot
    where TKey : IEquatable<TKey>
{
    void DeleteAsync(TEntity entity);
    void DeleteAsync(IEnumerable<TEntity> entities);
}

public abstract partial class BaseGenericRepository<TEntity, TKey>
    where TKey : IEquatable<TKey>
    where TEntity : Entity<TKey>, IAggregateRoot
{
    public void DeleteAsync(TEntity entity) => Context.Set<TEntity>().Remove(entity);

    public void DeleteAsync(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            Context.Set<TEntity>().Remove(entity);
        }
    }
}