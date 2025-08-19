using Concepts.Crud.WebApi.Application.Models;

namespace Concepts.Crud.WebApi.Application.Queries;

public interface IRelationshipQueries
{
    Task<ICollection<ActivityRelationship>> GetActivityRelationshipAsync(Guid activityId, CancellationToken ct);
    Task<ActivityRelationship> GetActivityRelationshipById(Guid activityId, Guid id, CancellationToken ct);
}