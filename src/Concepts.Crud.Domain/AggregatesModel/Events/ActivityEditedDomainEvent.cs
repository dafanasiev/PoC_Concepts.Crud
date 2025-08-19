using Concepts.Crud.Domain.AggregatesModel.ActivityAggregate;

namespace Concepts.Crud.Domain.AggregatesModel.Events;

public class ActivityEditedDomainEvent(Activity activity)
:INotification;