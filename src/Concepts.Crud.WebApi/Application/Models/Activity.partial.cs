using Concepts.Crud.WebApi.Application.Queries;

namespace Concepts.Crud.WebApi.Application.Models;

partial class Activity
{
    internal static async Task<ICollection<Activity>> MakeCollection(
        IEntityRefHrefProvider entityRefHrefProvider,
        IEnumerable<Domain.AggregatesModel.ActivityAggregate.Activity> activityList,
        CancellationToken ct
    )
    {
        var rv = new List<Activity>();
        foreach (var activity in activityList)
        {
            rv.Add(await Make(entityRefHrefProvider, activity, ct));
        }
        return rv;
    }    
    
    internal static async Task<ICollection<Activity>> MakeCollection(
        IEntityRefHrefProvider entityRefHrefProvider,
        IAsyncEnumerable<Domain.AggregatesModel.ActivityAggregate.Activity> activityList,
        CancellationToken ct
    )
    {
        var rv = new List<Activity>();
        await foreach (var activity in activityList.WithCancellation(ct))
        {
            rv.Add(await Make(entityRefHrefProvider, activity, ct));
        }
        return rv;
    }
    
    internal static async Task<Activity> Make(
        IEntityRefHrefProvider entityRefHrefProvider,
        Domain.AggregatesModel.ActivityAggregate.Activity activity,
        CancellationToken ct
    )
    {
        var a = new Activity
        {
            Id = activity.Id,
            Code = activity.Code,
            ClassName = $"{activity.Name}__class_name",  //TODO: use IClassNameGenerator
            Name = activity.Name,
            Description = activity.Description ?? string.Empty,
            IsGroup = activity.IsGroup,
            Type = new()
            {
                Id = activity.Type.Id,
                Name = activity.Type.Name,
            },
            Relationship = []
        };

        foreach (var relation in activity.RelationshipList)
        {
            if (relation.GC != null)
            {
                continue;       //TODO: fixme (GCProblem)
            }

            var r = new RelationshipType
            {
                Id = relation.RelationshipType.Id,
                Name = relation.RelationshipType.Name,
                Description = relation.RelationshipType.Description ?? string.Empty,
            };

            var eRef = new EntityRef
            {
                Id = relation.EntityRef.Id,
                Name = relation.EntityRef.Name,
                ReferredClassName = relation.EntityRef.ReferredClassName,
            };
            eRef.Href = (await entityRefHrefProvider.Obtain(eRef, ct)).AbsoluteUri;

            a.Relationship.Add(new()
            {
                Id = r.Id,
                RelationshipType = r,
                TargetSpecification = eRef
            });
        }

        return a;
    }
}