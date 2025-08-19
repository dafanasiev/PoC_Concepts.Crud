using Concepts.Crud.Domain.AggregatesModel.ActivityAggregate;
using Concepts.Crud.Domain.AggregatesModel.ActivityRelationshipAggregate;
using Concepts.Crud.Domain.AggregatesModel.ActivityTypeAggregate;
using Concepts.Crud.Domain.AggregatesModel.ClientRequestAggregate;
using Concepts.Crud.Domain.AggregatesModel.EntityRefAggregate;
using Concepts.Crud.Domain.AggregatesModel.RelationshipTypeAggregate;
using Concepts.Crud.Domain.SeedWork;
#if false
using Concepts.Crud.Domain.AggregatesModel.ActivityGraphAggregate;
#endif

namespace Concepts.Crud.Infrastructure;

public interface ICrudContext
    : IContext,
        IUnitOfWork
{
    IQueryable<ActivityType> ActivityTypeSet { get; }
    IQueryable<RelationshipType> RelationshipTypeSet { get; }
    IMutableQueryable<Activity> ActivitySet { get; }
    IMutableQueryable<ClientRequest> ClientRequestSet { get; }
    IQueryable<EntityRef> EntityRefSet { get; }
    IQueryable<ActivityRelationship> ActivityRelationshipSet { get; }

#if false
    IQueryable<ActivityGraph> ActivityGraphSet { get;  }
#endif
}