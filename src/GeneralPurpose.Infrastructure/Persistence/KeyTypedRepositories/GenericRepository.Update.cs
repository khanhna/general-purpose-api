using GeneralPurpose.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace GeneralPurpose.Infrastructure.Persistence.KeyTypedRepositories;

public interface IGenericRepository_Update<TEntity, in TKey>
    where TKey : IEquatable<TKey>
    where TEntity : Entity<TKey>, IAggregateRoot
{
    void UpdateAsync(TEntity entity);
    void UpdateAsync(IEnumerable<TEntity> entities);
}

public abstract partial class BaseGenericRepository<TEntity, TKey>
    where TKey : IEquatable<TKey>
    where TEntity : Entity<TKey>, IAggregateRoot
{
    public void UpdateAsync(TEntity entity)
    {
        entity.SetLastUpdatedTime();
        Context.Entry(entity).State = EntityState.Modified;
    }

    public void UpdateAsync(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            entity.SetLastUpdatedTime();
            Context.Entry(entity).State = EntityState.Modified;
        }
    }
}