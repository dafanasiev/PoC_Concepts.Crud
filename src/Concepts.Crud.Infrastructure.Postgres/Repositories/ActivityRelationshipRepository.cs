using Concepts.Crud.Domain.AggregatesModel.ActivityRelationshipAggregate;

namespace Concepts.Crud.Infrastructure.Postgres.Repositories;

public class ActivityRelationshipRepository(ICrudContext context)
    : IActivityRelationshipRepository
{
    public async Task<ActivityRelationship?> FindBy(Guid activityId, Guid relationshipTypeId, Guid entityRefId, CancellationToken ct)
    {
        return await context.ActivityRelationshipSet
            .Where(x => x.ActivityId == activityId
                        && x.RelationshipTypeId == relationshipTypeId
                        && x.EntityRefId == entityRefId)
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);
    }
}