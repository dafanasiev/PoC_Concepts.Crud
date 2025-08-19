using Concepts.Crud.Domain.SeedWork;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Concepts.Crud.Infrastructure.Postgres;

public class SoftDeleteInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
        {
            return base.SavingChangesAsync(
                eventData, result, cancellationToken);
        }

        IEnumerable<EntityEntry<Entity>> entries =
            eventData
                .Context
                .ChangeTracker
                .Entries<Entity>()
                .Where(e => e.State == EntityState.Deleted);

        foreach (EntityEntry<Entity> softDeletable in entries)
        {
            softDeletable.State = EntityState.Modified;
            softDeletable.Entity.GC = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}