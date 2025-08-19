using Concepts.Crud.Domain.AggregatesModel.ActivityAggregate;

namespace Concepts.Crud.Domain.AggregatesModel.ActivityRelationshipAggregate;

public interface IActivityRelationshipRepository
{
    Task<ActivityRelationship?> FindBy(Guid activityId, Guid relationshipTypeId, Guid entityRefId, CancellationToken ct);
}