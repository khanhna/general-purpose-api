using GeneralPurpose.Domain.SeedWork;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace GeneralPurpose.Infrastructure.Extensions;

public static class MediatorExtensions
{
    public static async Task DispatchDomainEventsAsync(this IMediator mediator, DbContext ctx)
    {
        var domainEntities = ctx.ChangeTracker
            .Entries<BaseEntity>()
            .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Count != 0)
            .ToArray();
        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents ?? [])
            .ToList();
        domainEntities
            .ToList()
            .ForEach(entity => entity.Entity.ClearDomainEvents());
        var tasks = domainEvents
            .Select(async (domainEvent) =>
            {
                await mediator.Publish(domainEvent);
            });
        
        await Task.WhenAll(tasks);
    }
}