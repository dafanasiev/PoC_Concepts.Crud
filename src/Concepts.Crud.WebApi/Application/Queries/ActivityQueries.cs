using Concepts.Crud.Infrastructure;
using Concepts.Crud.WebApi.Application.Models;
using Microsoft.EntityFrameworkCore;
#if false
using Concepts.Crud.WebApi.Application.Models;
#endif

namespace Concepts.Crud.WebApi.Application.Queries;

public class ActivityQueries(
    ICrudContext context,
    IEntityRefHrefProvider entityRefHrefProvider
)
    : IActivityQueries
{
    public async Task<ICollection<Activity>> GetActivity(
        IActivityQueries.GetActivityFilter filter,
        CancellationToken ct
    )
    {
        var alist = context.ActivitySet
            .Where(x => (filter.code == null || x.Code == filter.code)
                        && (filter.name == null || x.Name == filter.name)
                        && (filter.description == null ||
                            (x.Description != null && x.Description.Contains(filter.description!)))
            )
            .Include(x => x.Type)
            .Include(x => x.RelationshipList)
            .ThenInclude(x => x.RelationshipType)
            .Include(x => x.RelationshipList)
            .ThenInclude(x => x.EntityRef)
            .AsNoTracking()
            .AsAsyncEnumerable();

        var rv = await Activity.MakeCollection(entityRefHrefProvider, alist, ct);
        return rv;
    }

    public async Task<ICollection<Activity>> GetActivity(List<Guid> idList, CancellationToken ct)
    {
        var alist = context.ActivitySet
            .Where(x => x.GC == null
                        && idList.Contains(x.Id)
            )
            .Include(x => x.Type)
            .Include(x => x.RelationshipList)
            .ThenInclude(x => x.RelationshipType)
            .Include(x => x.RelationshipList)
            .ThenInclude(x => x.EntityRef)
            .AsNoTracking()
            .AsAsyncEnumerable();

        var rv = await Activity.MakeCollection(entityRefHrefProvider, alist, ct);
        if (rv.Count != idList.Count)
        {
            //TODO: better exception (database corrupted?) 
            throw new KeyNotFoundException(
                "The number of activity records does not match the number of activity request");
        }

        return rv;
    }


    public async Task<Activity> GetActivityById(Guid id, CancellationToken ct)
    {
        var activity = await context.ActivitySet
            .Include(x => x.Type)
            .Include(x => x.RelationshipList)
            .ThenInclude(x => x.RelationshipType)
            .Include(x => x.RelationshipList)
            .ThenInclude(x => x.EntityRef)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.GC == null && x.Id == id, ct);

        if (activity == null)
        {
            throw new KeyNotFoundException($"Activity with id {id} not found");
        }

        var rv = await Models.Activity.Make(entityRefHrefProvider, activity, ct);
        return rv;
    }

#if false
    public async Task<ICollection<ActivityGraph>> GetActivityGraph(CancellationToken ct)
    {
        var rv = new List<ActivityGraph>();
        await foreach (var activityGraph in context
                           .ActivityGraphSet
                           .AsNoTrackingWithIdentityResolution()
                           .Include(x => x.ActivityRelationshipList)
                           .ThenInclude(x => x.RelationshipType)
                           .Include(x => x.ActivityRelationshipList)
                           .ThenInclude(x => x.EntityRef)
                           .AsAsyncEnumerable()
                           .WithCancellation(ct))
        {
            var ag = new ActivityGraph
            {
                Id = activityGraph.Id,
                Code = activityGraph.Code,
                IsGroup = activityGraph.IsGroup,
                Name = activityGraph.Name,
                Type = activityGraph.Type,
                Relationship = []
            };

            foreach (var activityRelationship in activityGraph.ActivityRelationshipList)
            {
                var ar = await ActivityRelationship.Make(entityRefHrefProvider, activityRelationship, ct);
                ag.Relationship.Add(ar);
            }

            rv.Add(ag);
        }

        return rv;
    }
#endif
}