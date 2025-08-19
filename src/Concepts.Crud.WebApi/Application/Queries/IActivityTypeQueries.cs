using Concepts.Crud.WebApi.Application.Models;

namespace Concepts.Crud.WebApi.Application.Queries;

public interface IActivityTypeQueries
{
    Task<ICollection<ActivityType>> GetActivityTypeAsync(CancellationToken ct);
}