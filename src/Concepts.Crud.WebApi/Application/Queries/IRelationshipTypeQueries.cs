using Concepts.Crud.WebApi.Application.Models;

namespace Concepts.Crud.WebApi.Application.Queries;

public interface IRelationshipTypeQueries
{
    Task<ICollection<RelationshipType>> GetActivityRelationshipType(CancellationToken ct);
}