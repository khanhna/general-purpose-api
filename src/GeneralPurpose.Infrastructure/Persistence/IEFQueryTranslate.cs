using GeneralPurpose.Domain.SeedWork;

namespace GeneralPurpose.Infrastructure.Persistence;

public interface IEFQueryTranslate<TEntity> where TEntity : IAggregateRoot
{
    IQueryable<TEntity> TranslateToQuery(IQueryable<TEntity> query);
}
