using Concepts.Crud.WebApi.Application.Queries;
using Concepts.Crud.WebApi.Infrastructure.Services;

namespace Concepts.Crud.WebApi.Api.ActivityType;

public class ActivityTypeServices(
    IActivityTypeQueries queries
    ,ILogger<ActivityTypeServices> logger
    )
{
    public ILogger<ActivityTypeServices> Logger { get; } = logger;
    public IActivityTypeQueries Queries { get; } = queries;
}