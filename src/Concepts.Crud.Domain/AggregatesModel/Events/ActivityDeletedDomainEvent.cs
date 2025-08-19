using Concepts.Crud.Domain.AggregatesModel.ActivityAggregate;

namespace Concepts.Crud.Domain.AggregatesModel.Events;

// ReSharper disable once NotAccessedPositionalProperty.Global
public record ActivityDeletedDomainEvent(Activity Activity)
:INotification;