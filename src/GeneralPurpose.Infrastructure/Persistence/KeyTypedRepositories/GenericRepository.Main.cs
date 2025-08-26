using GeneralPurpose.Domain.SeedWork;
using GeneralPurpose.Infrastructure.Data;
using GeneralPurpose.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace GeneralPurpose.Infrastructure.Persistence.KeyTypedRepositories;

public interface IRepository<TEntity, in TKey> :
    IReadOnlyRepository<TEntity, TKey>,
    IGenericRepository_Create<TEntity, TKey>,
    IGenericRepository_Update<TEntity, TKey>,
    IGenericRepository_Delete<TEntity, TKey>
    where TKey : IEquatable<TKey>
    where TEntity : Entity<TKey>, IAggregateRoot
{
    IUnitOfWork UnitOfWork { get; }
}

public abstract partial class BaseGenericRepository<TEntity, TKey> : IRepository<TEntity, TKey>
    where TKey : IEquatable<TKey>
    where TEntity : Entity<TKey>, IAggregateRoot
{
    protected readonly DbContext Context;
    private readonly IIdentityService _identityService;

    public BaseGenericRepository(DbContext context, IIdentityService identityService)
    {
        Context = context;
        _identityService = identityService;
    }

    public virtual IUnitOfWork UnitOfWork => throw new ArgumentException("Db context should be implemented down sub class!");
}