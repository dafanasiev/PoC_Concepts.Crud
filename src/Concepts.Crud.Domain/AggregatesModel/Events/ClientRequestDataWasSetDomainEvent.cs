using Concepts.Crud.Domain.AggregatesModel.ClientRequestAggregate;

namespace Concepts.Crud.Domain.AggregatesModel.Events;

public class ClientRequestDataWasSetDomainEvent(ClientRequest request)
    : INotification;