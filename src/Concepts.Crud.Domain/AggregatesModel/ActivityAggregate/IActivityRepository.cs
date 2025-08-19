using Concepts.Crud.Domain.SeedWork;

namespace Concepts.Crud.Domain.AggregatesModel.ActivityAggregate;

public interface IActivityRepository
    :IRepository<Activity>
{
    Task<ICollection<Activity>> AddRange(ICollection<Activity> items, CancellationToken ct);
    Task<Activity?> Find(Guid id, CancellationToken ct);
}