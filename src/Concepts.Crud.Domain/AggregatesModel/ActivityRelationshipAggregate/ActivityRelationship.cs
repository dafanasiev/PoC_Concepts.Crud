using Concepts.Crud.Domain.AggregatesModel.ActivityAggregate;
using Concepts.Crud.Domain.AggregatesModel.EntityRefAggregate;
using Concepts.Crud.Domain.AggregatesModel.RelationshipTypeAggregate;
using Concepts.Crud.Domain.SeedWork;
#if false
using Concepts.Crud.Domain.AggregatesModel.ActivityGraphAggregate;
#endif

namespace Concepts.Crud.Domain.AggregatesModel.ActivityRelationshipAggregate;

public class ActivityRelationship
    : Entity<Guid>, IAggregateRoot
{
    public Guid ActivityId { get; private set; }
    public Guid RelationshipTypeId { get; private set; }
    public Guid EntityRefId { get; private set; }

    public virtual Activity Activity { get; private set; }
    public virtual RelationshipType RelationshipType { get; private set; }
    public virtual EntityRef EntityRef { get; private set; }

#if false
    public Guid ActivityGraphId { get; private set; }
    public virtual ActivityGraph ActivityGraph { get; private set; }
#endif

    protected ActivityRelationship()
    {
    }

    public ActivityRelationship(Activity activity, RelationshipType relationshipType, EntityRef entityRef
        #if false
        , ActivityGraph activityGraph
        #endif
        )
        : this()
    {
        Activity = activity;
        ActivityId = activity.Id;

        RelationshipType = relationshipType;
        RelationshipTypeId = relationshipType.Id;

        EntityRef = entityRef;
        EntityRefId = entityRef.Id;
#if false
        ActivityGraph = activityGraph;
        ActivityGraphId = activityGraph.Id;
#endif
    }
}