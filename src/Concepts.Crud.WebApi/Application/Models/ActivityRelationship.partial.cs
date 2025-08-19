using Concepts.Crud.WebApi.Application.Queries;

namespace Concepts.Crud.WebApi.Application.Models;

partial class ActivityRelationship
{
    internal static async Task<ActivityRelationship> Make(
        IEntityRefHrefProvider entityRefHrefProvider,
        Domain.AggregatesModel.ActivityRelationshipAggregate.ActivityRelationship activityRelationship,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(entityRefHrefProvider);
        ArgumentNullException.ThrowIfNull(activityRelationship);
        
        var r = new ActivityRelationship();
        r = new ActivityRelationship
        {
            Id = activityRelationship.Id,
        };

        var relationshipType = activityRelationship.RelationshipType;
        r.RelationshipType = new RelationshipType
        {
            Id = relationshipType.Id,
            Name = relationshipType.Name,
            Description = relationshipType.Description,
        };


        var entityRef = activityRelationship.EntityRef;
        r.TargetSpecification = new EntityRef
        {
            Id = entityRef.Id,
            Name = entityRef.Name,
            ReferredClassName = entityRef.ReferredClassName
        };
        r.TargetSpecification.Href = (await entityRefHrefProvider.Obtain(r.TargetSpecification, ct)).AbsoluteUri;
        return r;
    }
}