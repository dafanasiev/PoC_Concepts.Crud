using Concepts.Crud.Domain.AggregatesModel.ActivityAggregate;
using Concepts.Crud.Domain.AggregatesModel.ActivityTypeAggregate;
using Concepts.Crud.Domain.AggregatesModel.EntityRefAggregate;
using Concepts.Crud.Domain.AggregatesModel.RelationshipTypeAggregate;
using Concepts.Crud.Infrastructure.Idempotency;
using Concepts.Crud.WebApi.Application.Queries;
using Activity = Concepts.Crud.WebApi.Application.Models.Activity;

namespace Concepts.Crud.WebApi.Application.Commands;

public class CreateActivityCommandHandler
    : IRequestHandler<CreateActivityCommand, ICollection<Activity>>
{
    private readonly IActivityRepository _activityRepository;
    private readonly IActivityTypeRepository _activityTypeRepository;
    private readonly IRelationshipTypeRepository _relationshipTypeRepository;
    private readonly IEntityRefRepository _entityRefRepository;
    private readonly IEntityRefHrefProvider _entityRefHrefProvider;
    private readonly ILogger<CreateActivityCommandHandler> _logger;

    public CreateActivityCommandHandler(
        IActivityRepository activityRepository,
        IActivityTypeRepository activityTypeRepository,
        IRelationshipTypeRepository relationshipTypeRepository,
        IEntityRefRepository entityRefRepository,
        IEntityRefHrefProvider entityRefHrefProvider,
        ILogger<CreateActivityCommandHandler> logger
    )
    {
        _activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
        _activityTypeRepository = activityTypeRepository;
        _relationshipTypeRepository = relationshipTypeRepository;
        _entityRefRepository = entityRefRepository;
        _entityRefHrefProvider = entityRefHrefProvider;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        //TODO: add IActivityIntegrationEventsService
    }

    public async Task<ICollection<Activity>> Handle(CreateActivityCommand m, CancellationToken ct)
    {
        try
        {
            var alist = new List<Concepts.Crud.Domain.AggregatesModel.ActivityAggregate.Activity>();
            foreach (var mItem in m.Data)
            {
                var a = await mItem.ToActivity(
                    _activityTypeRepository,
                    _relationshipTypeRepository,
                    _entityRefRepository,
                    ct);
                alist.Add(a);
            }

            _ = await _activityRepository.AddRange(alist, ct);

            if (!await _activityRepository.UnitOfWork.SaveEntitiesAsync(ct))
            {
                //TODO: better exceptions?
                throw new Exception("Failed to save activities");
            }

            var rv = await Activity.MakeCollection(_entityRefHrefProvider, alist, ct);
            //TOOD: add IActivityIntegrationEventsService.AddAndSaveEventAsync(new ActivitySavedIntegrationEvent(activitiesAggList))
            return rv;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating activity");
            //TODO: better exceptions?
            throw;
        }
    }

    // Use for Idempotency in Command process
    public class IdentifiedCommandHandler(
        IMediator mediator,
        IRequestManager requestManager,
        IActivityQueries activityQueries,
        ILogger<IdentifiedCommandHandler<CreateActivityCommand, ICollection<Activity>, List<Guid>>> logger
    )
        : IdentifiedCommandHandler<CreateActivityCommand, ICollection<Activity>, List<Guid>>(mediator, requestManager, logger)
    {
        protected override async Task<ICollection<Activity>> CreateResultForDuplicateRequest(
            List<Guid> data,
            CancellationToken ct
        )
        {
            ICollection<Activity> alist;
            try
            {
                alist = await activityQueries.GetActivity(data, ct);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error querying activity list");
                throw;
            }

            return alist;
        }

        public override async Task<ICollection<Activity>>
            Handle(
                IdentifiedCommand<CreateActivityCommand, ICollection<Activity>> message,
                CancellationToken ct
            )
        {
            var rv = await base.Handle(message, ct);
            var savedIds = rv.Select(x => x.Id).ToList();
            try
            {
                await RequestManager.AcknowledgeAsync(message.Id, savedIds, ct);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "RequestManager unable to save activity ids");
                throw;
            }

            return rv;
        }
    }
}