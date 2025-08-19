using Concepts.Crud.Infrastructure;
using Concepts.Crud.WebApi.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Concepts.Crud.WebApi.Application.Queries;

public class ActivityTypeQueries(ICrudContext context) : IActivityTypeQueries
{
    public async Task<ICollection<ActivityType>> GetActivityTypeAsync(CancellationToken ct)
    {
        return await context.ActivityTypeSet
            .Select(c => new ActivityType
            {
                Id = c.Id,
                Name = c.Name,
            })
            .AsNoTracking()
            .ToListAsync(ct);
    }
}