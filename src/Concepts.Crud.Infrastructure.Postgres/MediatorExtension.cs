using MediatR;
using Concepts.Crud.Domain.SeedWork;

namespace Concepts.Crud.Infrastructure.Postgres;

static class MediatorExtension
{
    public static async Task DispatchDomainEventsAsync(this IMediator mediator, BaseContext ctx)
    {
        var domainEntities = ctx.ChangeTracker
            .Entries<Entity>()
            .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any())
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents!)
            .ToList();

        domainEntities
            .ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
            await mediator.Publish(domainEvent);
    }
}