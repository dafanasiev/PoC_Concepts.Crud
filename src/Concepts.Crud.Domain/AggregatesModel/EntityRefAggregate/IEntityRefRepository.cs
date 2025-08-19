namespace Concepts.Crud.Domain.AggregatesModel.EntityRefAggregate;

public interface IEntityRefRepository
{
    Task<EntityRef?> FindById(Guid id, CancellationToken ct);
}