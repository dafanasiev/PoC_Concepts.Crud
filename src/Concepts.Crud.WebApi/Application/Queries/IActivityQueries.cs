#if false
using Concepts.Crud.WebApi.Application.Models;
#endif

namespace Concepts.Crud.WebApi.Application.Queries;

public interface IActivityQueries
{
    public record GetActivityFilter(
        string? description,
        string? name,
        string? code
    );
    
    Task<ICollection<Models.Activity>> GetActivity(GetActivityFilter filter, CancellationToken ct);

    Task<Models.Activity> GetActivityById(Guid id, CancellationToken ct);

    Task<ICollection<Models.Activity>> GetActivity(List<Guid> idList, CancellationToken ct);
    
#if false
    Task<ICollection<ActivityGraph>> GetActivityGraph(CancellationToken ct);
#endif
}