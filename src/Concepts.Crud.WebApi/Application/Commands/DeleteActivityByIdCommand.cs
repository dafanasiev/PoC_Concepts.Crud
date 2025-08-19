namespace Concepts.Crud.WebApi.Application.Commands;

public record DeleteActivityByIdCommand(Guid ActivityId)
    : IRequest<bool>;