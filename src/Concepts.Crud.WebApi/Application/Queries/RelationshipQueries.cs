using Concepts.Crud.Infrastructure;
using Concepts.Crud.WebApi.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Concepts.Crud.WebApi.Application.Queries;

public class RelationshipQueries(ICrudContext context, IEntityRefHrefProvider entityRefHrefProvider) : IRelationshipQueries
{
    public async Task<ICollection<ActivityRelationship>> GetActivityRelationshipAsync(Guid activityId, CancellationToken ct)
    {
        var activity = await context
            .ActivitySet
            .Where(x => x.GC == null)
            .AsNoTracking()
            .AnyAsync(x => x.Id == activityId, ct);

        if (!activity)
        {
            throw new KeyNotFoundException($"activity {activityId} not found"); //TODO: exception types
        }

        var rv = new List<ActivityRelationship>();
        await foreach (var activityRelationship in context
                           .ActivityRelationshipSet
                           .Where(x => x.GC == null && x.ActivityId == activityId)
                           .Include(x => x.RelationshipType)
                           .Include(x => x.EntityRef)
                           .AsNoTracking()
                           .AsAsyncEnumerable()
                           .WithCancellation(ct))
        {
            var r = await ActivityRelationship.Make(entityRefHrefProvider, activityRelationship, ct);
            rv.Add(r);
        }

        return rv;
    }

    public async Task<ActivityRelationship> GetActivityRelationshipById(Guid activityId, Guid id, CancellationToken ct)
    {
        var activity = await context.ActivitySet
            .AnyAsync(x => x.GC == null && x.Id == activityId, ct);
        if (!activity)
        {
            throw new KeyNotFoundException($"activity {activityId} not found"); //TODO: exception types
        }

        var activityRelationshipList = await context
            .ActivityRelationshipSet
            .Include(x => x.RelationshipType)
            .Include(x => x.EntityRef)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.GC == null && x.Id == id && x.ActivityId == activityId, ct);

        if (activityRelationshipList == null)
        {
            throw new KeyNotFoundException($"activityRelationship {id} not found for activity {activityId}"); //TODO: exception types
        }

        var rv = await ActivityRelationship.Make(entityRefHrefProvider, activityRelationshipList, ct);
        return rv;
    }
}