using System.Data;
using System.Reflection;
using GeneralPurpose.Domain.Entities;
using GeneralPurpose.Domain.SeedWork;
using GeneralPurpose.Infrastructure.Extensions;
using GeneralPurpose.Infrastructure.Services;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage;

namespace GeneralPurpose.Infrastructure.Data;

public enum DbContextSaveChangeBehavior
{
    Default,
    BypassDefault,
    BypassAllAdditionalProcess
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default,
        DbContextSaveChangeBehavior saveChangeBehavior = DbContextSaveChangeBehavior.Default,
        Guid? assignmentUserIdentity = null, params Action<DbContext>[] ctxAction);

    /// <summary>
    /// This method was use to dispatch Domain events if any before call base SaveChangesAsync
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// /// <param name="assignmentUserIdentity"></param>
    /// /// /// <param name="saveChangeBehavior"></param>
    /// <param name="ctxAction"></param>
    /// <returns></returns>
    Task<bool> SaveEntitiesAsync<TEntity, TKey>(CancellationToken cancellationToken = default,
        DbContextSaveChangeBehavior saveChangeBehavior = DbContextSaveChangeBehavior.Default,
        Guid? assignmentUserIdentity = null, params Action<DbContext>[] ctxAction)
        where TEntity : Entity<TKey> where TKey : IEquatable<TKey>;

    void DetachAllEntities();
}

public class ApplicationDbContext : DbContext, IUnitOfWork
{
    private readonly IMediator _mediator;
    private readonly IIdentityService _identityService;
    private IDbContextTransaction? _currentTransaction;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IMediator mediator,
        IIdentityService identityService) : base(options)
    {
        _mediator = mediator;
        _identityService = identityService;
    }

    public IDbContextTransaction GetCurrentTransaction()
    {
        return _currentTransaction;
    }

    public bool HasActiveTransaction => _currentTransaction != null;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<AppSystem>().HasMany(x => x.WorkingUnits).WithOne(x => x.AppSystem)
            .HasForeignKey(x => x.SystemId);
        modelBuilder.Entity<AppSystem>().HasMany(x => x.ImageCompositionConfigs).WithOne(x => x.AppSystem)
            .HasForeignKey(x => x.AppSystemId);

        modelBuilder.Entity<WorkingUnit>().HasMany(x => x.Transactions).WithOne(x => x.WorkingUnit)
            .HasForeignKey(x => x.WorkingUnitId);
        modelBuilder.Entity<WorkingUnit>().HasOne(x => x.ImageVintageProcessConfig).WithOne(x => x.WorkingUnit)
            .HasForeignKey<ImageVintageProcessConfig>(x => x.WorkingUnitId);

        base.OnModelCreating(modelBuilder);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default,
        DbContextSaveChangeBehavior saveChangeBehavior = DbContextSaveChangeBehavior.Default,
        Guid? assignmentUserIdentity = null, params Action<DbContext>[] ctxAction)
    {
        if (ctxAction == null) return base.SaveChangesAsync(cancellationToken);
        foreach (var item in ctxAction) item?.Invoke(this);

        switch (saveChangeBehavior)
        {
            case DbContextSaveChangeBehavior.Default:
                OnBeforeSaveChanges(assignmentUserIdentity);
                break;
            case DbContextSaveChangeBehavior.BypassDefault:
                OnBeforeSaveChangesDefault();
                break;
            case DbContextSaveChangeBehavior.BypassAllAdditionalProcess:
                break;
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> SaveEntitiesAsync<TEntity, TKey>(CancellationToken cancellationToken = default,
        DbContextSaveChangeBehavior saveChangeBehavior = DbContextSaveChangeBehavior.Default,
        Guid? assignmentUserIdentity = null,
        params Action<DbContext>[] ctxAction) where TEntity : Entity<TKey> where TKey : IEquatable<TKey>
    {
        if (ctxAction != null)
            foreach (var item in ctxAction)
                item?.Invoke(this);

        // Dispatch Domain Events collection. 
        // Choices:
        // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
        // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
        // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
        // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
        await _mediator.DispatchDomainEventsAsync(this);

        switch (saveChangeBehavior)
        {
            case DbContextSaveChangeBehavior.Default:
                OnBeforeSaveChanges(assignmentUserIdentity);
                break;
            case DbContextSaveChangeBehavior.BypassDefault:
                OnBeforeSaveChangesDefault();
                break;
            case DbContextSaveChangeBehavior.BypassAllAdditionalProcess:
                break;
        }

        // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
        // performed through the DbContext will be committed
        _ = await base.SaveChangesAsync(cancellationToken);

        return true;
    }

    public void DetachAllEntities()
    {
        var changedEntriesCopy = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added ||
                        e.State == EntityState.Modified ||
                        e.State == EntityState.Deleted)
            .ToList();

        foreach (var entry in changedEntriesCopy)
            entry.State = EntityState.Detached;
    }

    private void OnBeforeSaveChanges(Guid? assignmentUserIdentity = null)
    {
        var userIdentity = assignmentUserIdentity ?? _identityService.GetUserIdentity();
        var changedEntries = ChangeTracker.Entries<BaseEntity>()
            .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);

        foreach (var item in changedEntries)
            switch (item.State)
            {
                case EntityState.Added:
                    item.Entity.SetCreatedTime(userIdentity);
                    break;
                case EntityState.Modified:
                    item.Entity.SetLastUpdatedTime(userIdentity);
                    break;
            }
    }

    private void OnBeforeSaveChangesDefault()
    {
        var changedEntries = ChangeTracker.Entries<BaseEntity>()
            .Where(x => x.State == EntityState.Modified);

        foreach (var item in changedEntries) item.Entity.SetLastUpdatedTime();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        if (_currentTransaction != null) return null;

        _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        return _currentTransaction;
    }

    public async Task CommitTransactionAsync(IDbContextTransaction transaction)
    {
        if (transaction == null) throw new ArgumentNullException(nameof(transaction));

        if (transaction != _currentTransaction)
            throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

        try
        {
            await SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await RollbackTransaction();
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }

    public async Task RollbackTransaction()
    {
        try
        {
            await _currentTransaction?.RollbackAsync();
        }
        finally
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }
}

public class ApplicationDbContextDesignTimeFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql("Server=172.25.157.236;Port=5432;Database=general_purpose;User Id=admin;Password=p@ssWoRd!;");

        return new ApplicationDbContext(optionsBuilder.Options, null!, null!);
    }
}