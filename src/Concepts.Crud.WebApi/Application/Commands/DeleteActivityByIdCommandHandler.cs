using System.Diagnostics;
using Concepts.Crud.Domain.AggregatesModel.ActivityAggregate;
using Concepts.Crud.Infrastructure.Idempotency;

namespace Concepts.Crud.WebApi.Application.Commands;

public class DeleteActivityByIdCommandHandler
    : IRequestHandler<DeleteActivityByIdCommand, bool>
{
    private readonly IActivityRepository _activityRepository;
    private readonly ILogger<DeleteActivityByIdCommandHandler> _logger;

    public DeleteActivityByIdCommandHandler(
        IActivityRepository activityRepository,
        ILogger<DeleteActivityByIdCommandHandler> logger)
    {
        _activityRepository = activityRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteActivityByIdCommand m, CancellationToken ct)
    {
        try
        {
            var a = await _activityRepository.Find(m.ActivityId, ct);
            if (a == null)
            {
                //TODO: better exceptions?
                throw new KeyNotFoundException($"Activity {m.ActivityId:D} Not Found");
            }

            a.Delete();

            if (1 != await _activityRepository.UnitOfWork.SaveChangesAsync(ct))
            {
                //TODO: better exceptions?
                throw new InvalidOperationException("Bad logic"); //TODO: fixme
            }

            //TOOD: add IActivityIntegrationEventsService.AddAndSaveEventAsync(new ActivityDeletedIntegrationEvent(id))
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting activity");
            //TODO: better exceptions?
            throw;
        }
    }

    public class IdentifiedCommandHandler(
        IMediator mediator,
        IRequestManager requestManager,
        ILogger<IdentifiedCommandHandler<DeleteActivityByIdCommand, bool, Guid>> logger
    )
        : IdentifiedCommandHandler<DeleteActivityByIdCommand, bool, Guid>(mediator, requestManager, logger)
    {
        protected override async Task<bool> CreateResultForDuplicateRequest(Guid data, CancellationToken ct)
        {
            //TODO: better logging
            Logger.LogInformation($"Duplicate command {nameof(DeleteActivityByIdCommand)} request ignored");
            return true; // ignore duplicate requests
        }
    }
}