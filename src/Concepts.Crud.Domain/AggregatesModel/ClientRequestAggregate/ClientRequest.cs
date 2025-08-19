using System.ComponentModel.DataAnnotations;
using Concepts.Crud.Domain.AggregatesModel.Events;
using Concepts.Crud.Domain.Exceptions;
using Concepts.Crud.Domain.SeedWork;

namespace Concepts.Crud.Domain.AggregatesModel.ClientRequestAggregate;

public class ClientRequest
    : SimpleEntity<Guid>
{
    [Required] public string Name { get; private set; }
    public DateTime Time { get; private set; }

    public string? Data { get; private set; }

    protected ClientRequest()
    {
    }

    public ClientRequest(Guid id, string name, DateTime time, string? data)
    {
        Id = id;
        Name = name;
        Time = time;
        Data = data;
    }

    public void SetData(string? data)
    {
        if (Data != null)
            throw new CrudDomainException($"{nameof(Data)} for {nameof(ClientRequest)} already set");

        Data = data;
        AddDomainEvent(new ClientRequestDataWasSetDomainEvent(this));
    }
}