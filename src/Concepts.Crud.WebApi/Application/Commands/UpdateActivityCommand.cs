using Concepts.Crud.WebApi.Application.Models;

namespace Concepts.Crud.WebApi.Application.Commands;

public record UpdateActivityCommand(IUpdateActivityCommandData Data)
    : IRequest<Activity>;

public interface IUpdateActivityCommandData : IUpsertActivityCommandData
{
    Guid Id { get; }
}