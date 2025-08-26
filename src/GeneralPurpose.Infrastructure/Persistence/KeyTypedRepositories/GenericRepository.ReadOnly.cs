using System.Linq.Expressions;
using GeneralPurpose.Domain.SeedWork;
using GeneralPurpose.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace GeneralPurpose.Infrastructure.Persistence.KeyTypedRepositories;

public interface IReadOnlyRepository<TEntity, in TKey>
    where TKey : IEquatable<TKey>
    where TEntity : Entity<TKey>, IAggregateRoot
{
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> criteria,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? includeCondition = null,
        bool isNoTracking = false, CancellationToken cancellationToken = default);
    Task<TEntity?> FirstOrDefaultAsync(bool isNoTracking = false, CancellationToken cancellationToken = default);

    Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> criteria,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? includeCondition = null,
        bool isNoTracking = false, CancellationToken cancellationToken = default);
    Task<TEntity> SingleAsync(bool isNoTracking = false, CancellationToken cancellationToken = default);

    Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> criteria,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? includeCondition = null,
        bool isNoTracking = false, CancellationToken cancellationToken = default);
    Task<TEntity?> SingleOrDefaultAsync(bool isNoTracking = false, CancellationToken cancellationToken = default);

    Task<List<TEntity>> ListAsync(Expression<Func<TEntity, bool>> criteria,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? includeCondition = null,
        bool isNoTracking = false, CancellationToken cancellationToken = default);

    Task<List<TEntity>> ListAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>>? includeCondition = null,
        bool isNoTracking = false, CancellationToken cancellationToken = default);

    IQueryable<TEntity> ExposeQueryable(bool isNoTracking = false);
}

public abstract partial class BaseGenericRepository<TEntity, TKey>
    where TKey : IEquatable<TKey>
    where TEntity : Entity<TKey>, IAggregateRoot
{
    public Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> criteria,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? includeCondition = null,
        bool isNoTracking = false, CancellationToken cancellationToken = default)
    {
        var query = QueryableExtensions.AsNoTrackingIf<TEntity>(Context.Set<TEntity>().AsQueryable(), isNoTracking);
        query = includeCondition == null ? query : includeCondition.Invoke(query);
        return query.WhereIf(criteria != null, criteria!).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<TEntity?> FirstOrDefaultAsync(bool isNoTracking = false, CancellationToken cancellationToken = default)
        => QueryableExtensions.AsNoTrackingIf<TEntity>(Context.Set<TEntity>().AsQueryable(), isNoTracking).FirstOrDefaultAsync(cancellationToken);

    public Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> criteria, 
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? includeCondition = null, 
        bool isNoTracking = false, CancellationToken cancellationToken = default)
    {
        var query = QueryableExtensions.AsNoTrackingIf<TEntity>(Context.Set<TEntity>().AsQueryable(), isNoTracking);
        query = includeCondition == null ? query : includeCondition.Invoke(query);
        return query.WhereIf(criteria != default, criteria!).SingleAsync(cancellationToken);
    }

    public Task<TEntity> SingleAsync(bool isNoTracking = false, CancellationToken cancellationToken = default)
        => QueryableExtensions.AsNoTrackingIf<TEntity>(Context.Set<TEntity>().AsQueryable(), isNoTracking).SingleAsync(cancellationToken);

    public Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> criteria,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? includeCondition = null,
        bool isNoTracking = false, CancellationToken cancellationToken = default)
    {
        var query = QueryableExtensions.AsNoTrackingIf<TEntity>(Context.Set<TEntity>().AsQueryable(), isNoTracking);
        query = includeCondition == null ? query : includeCondition.Invoke(query);
        return query.WhereIf(criteria != default, criteria!).SingleOrDefaultAsync(cancellationToken);
    }

    public Task<TEntity?> SingleOrDefaultAsync(bool isNoTracking = false,CancellationToken cancellationToken = default)
        => Context.Set<TEntity>().AsQueryable().AsNoTrackingIf<TEntity>(isNoTracking).SingleOrDefaultAsync(cancellationToken);

    public Task<List<TEntity>> ListAsync(Expression<Func<TEntity, bool>> criteria,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? includeCondition = null,
        bool isNoTracking = false, CancellationToken cancellationToken = default)
    {
        var query = QueryableExtensions.AsNoTrackingIf<TEntity>(Context.Set<TEntity>().AsQueryable(), isNoTracking);
        query = includeCondition == null ? query : includeCondition.Invoke(query);
        return query.WhereIf(criteria != default, criteria!).ToListAsync(cancellationToken);
    }

    public Task<List<TEntity>> ListAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>>? includeCondition = null,
        bool isNoTracking = false, CancellationToken cancellationToken = default)
    {
        var query = QueryableExtensions.AsNoTrackingIf<TEntity>(Context.Set<TEntity>().AsQueryable(), isNoTracking);
        query = includeCondition == null ? query : includeCondition.Invoke(query);
        return query.ToListAsync(cancellationToken);
    }

    public IQueryable<TEntity> ExposeQueryable(bool isNoTracking = false) => QueryableExtensions.AsNoTrackingIf<TEntity>(Context.Set<TEntity>(), isNoTracking);
}