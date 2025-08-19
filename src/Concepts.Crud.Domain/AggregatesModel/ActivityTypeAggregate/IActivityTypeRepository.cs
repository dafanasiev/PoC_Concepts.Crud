namespace Concepts.Crud.Domain.AggregatesModel.ActivityTypeAggregate;

public interface IActivityTypeRepository
{
    Task<ActivityType?> FindById(Guid id, CancellationToken ct);
}