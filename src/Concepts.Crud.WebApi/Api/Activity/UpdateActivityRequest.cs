using Concepts.Crud.WebApi.Application.Commands;

namespace Concepts.Crud.WebApi.Api.Activity;

public class UpdateActivityRequest
    : UpsertActivityRequest,
        IUpdateActivityCommandData
{
    internal Guid? Id { get; set; }
    
    Guid IUpdateActivityCommandData.Id => Id ?? throw new InvalidOperationException("Id must be set before access this property");
}