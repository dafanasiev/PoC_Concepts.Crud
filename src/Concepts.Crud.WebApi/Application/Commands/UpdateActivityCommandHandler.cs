using Concepts.Crud.Domain.AggregatesModel.ActivityAggregate;
using Concepts.Crud.Domain.AggregatesModel.ActivityRelationshipAggregate;
using Concepts.Crud.Domain.AggregatesModel.ActivityTypeAggregate;
using Concepts.Crud.Domain.AggregatesModel.EntityRefAggregate;
using Concepts.Crud.Domain.AggregatesModel.RelationshipTypeAggregate;
using Concepts.Crud.Infrastructure.Idempotency;
using Concepts.Crud.WebApi.Application.Queries;
using Activity = Concepts.Crud.WebApi.Application.Models.Activity;

namespace Concepts.Crud.WebApi.Application.Commands;

public class UpdateActivityCommandHandler
    : IRequestHandler<UpdateActivityCommand, Activity>
{
    private readonly IActivityRepository _activityRepository;
    private readonly IActivityTypeRepository _activityTypeRepository;
    private readonly IRelationshipTypeRepository _relationshipTypeRepository;
    private readonly IEntityRefRepository _entityRefRepository;
    private readonly IEntityRefHrefProvider _entityRefHrefProvider;
    private readonly ILogger<UpdateActivityCommandHandler> _logger;

    public UpdateActivityCommandHandler(
        IActivityRepository activityRepository,
        IActivityTypeRepository activityTypeRepository,
        IRelationshipTypeRepository relationshipTypeRepository,
        IEntityRefRepository entityRefRepository,
        IEntityRefHrefProvider entityRefHrefProvider,
        ILogger<UpdateActivityCommandHandler> logger
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

    public async Task<Activity> Handle(UpdateActivityCommand m, CancellationToken ct)
    {
        try
        {
            var d = m.Data;

            var a = await _activityRepository.Find(d.Id, ct);
            if (a == null)
            {
                //TODO: better exceptions?
                throw new KeyNotFoundException("activity not found");
            }

            var at = await _activityTypeRepository.FindById(d.Type.Id, ct);
            if (at == null)
            {
                //TODO: better exceptions?
                throw new KeyNotFoundException("activity type not found");
            }

            var arl = new List<Domain.AggregatesModel.ActivityAggregate.Activity.SetRelationshipListArgItem>();
            foreach (var dr in d.Relationship)
            {
                var arlItem = new Domain.AggregatesModel.ActivityAggregate.Activity.SetRelationshipListArgItem();

                var art = await _relationshipTypeRepository.FindById(dr.RelationshipType.Id, ct);
                if (art == null)
                {
                    //TODO: better exceptions?
                    throw new KeyNotFoundException("relationship type not found");
                }

                var ats = await _entityRefRepository.FindById(dr.TargetSpecification.Id, ct);
                if (ats == null)
                {
                    //TODO: better exceptions?
                    throw new KeyNotFoundException("targetSpecification type not found");
                }

                arlItem.RelationshipType = art;
                arlItem.TargetSpecification = ats;
                arl.Add(arlItem);
            }

            try
            {
                a.BeginEdit();
                a.SetCode(d.Code);
                a.SetType(at);
                a.SetName(d.Name);
                a.SetDescription(d.Description);
                a.SetIsGroup(d.IsGroup);
                a.SetRelationshipList(arl);

                a.EndEdit();
            }
            catch (Exception e)
            {
                //TOOD: better log, better exceptions
                a.CancelEdit();
                _logger.LogError(e, "Error updating activity");
                throw;
            }

            await _activityRepository.UnitOfWork.SaveChangesAsync(ct);
            
            if (!await _activityRepository.UnitOfWork.SaveEntitiesAsync(ct))
            {
                //TODO: better exceptions?
                throw new Exception("Failed to save activity");
            }

            var rv = await Activity.Make(_entityRefHrefProvider, a, ct);
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
        ILogger<IdentifiedCommandHandler<UpdateActivityCommand, Activity, Guid>> logger
    )
        : IdentifiedCommandHandler<UpdateActivityCommand, Activity, Guid>(mediator, requestManager, logger)
    {
        protected override async Task<Activity> CreateResultForDuplicateRequest(
            Guid data,
            CancellationToken ct
        )
        {
            try
            {
                var a = await activityQueries.GetActivityById(data, ct);
                return a;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error querying activity {data}");
                throw;
            }
        }

        public override async Task<Activity>
            Handle(
                IdentifiedCommand<UpdateActivityCommand, Activity> message,
                CancellationToken ct
            )
        {
            var rv = await base.Handle(message, ct);
            try
            {
                await RequestManager.AcknowledgeAsync(message.Id, rv.Id, ct);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "RequestManager unable to save activity id");
                throw;
            }

            return rv;
        }
    }
}