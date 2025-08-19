using Concepts.Crud.WebApi.Application.Queries;

namespace Concepts.Crud.WebApi.Api.Activity;

public class ActivityServices(
    IActivityQueries queries,
    ILogger<ActivityServices> logger,
    IProblemHrefProvider problemHrefProvider,
    IMediator mediator
)
{
    public IActivityQueries Queries { get; } = queries;
    public ILogger<ActivityServices> Logger { get; } = logger;
    public IProblemHrefProvider ProblemHrefProvider { get; } = problemHrefProvider;
    public IMediator Mediator { get; } = mediator;
}