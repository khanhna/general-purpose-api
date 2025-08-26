using GeneralPurpose.Domain.SeedWork;
using GeneralPurpose.Infrastructure.Data;
using GeneralPurpose.Infrastructure.Services;

namespace GeneralPurpose.Infrastructure.Persistence;

public class BaseRepository<TEntity, TKey> : KeyTypedRepositories.BaseGenericRepository<TEntity, TKey>
    where TKey : IEquatable<TKey>
    where TEntity : Entity<TKey>, IAggregateRoot
{
    private readonly ApplicationDbContext _context;

    public BaseRepository(ApplicationDbContext context, IIdentityService identityService) : base(context, identityService)
    {
        _context = context;
    }

    public override IUnitOfWork UnitOfWork => _context;
}