namespace Concepts.Crud.Domain.AggregatesModel.RelationshipTypeAggregate;

public interface IRelationshipTypeRepository
{
    Task<RelationshipType?> FindById(Guid id, CancellationToken ct);
}