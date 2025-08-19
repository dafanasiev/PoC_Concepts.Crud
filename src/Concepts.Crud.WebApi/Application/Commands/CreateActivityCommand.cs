using Concepts.Crud.WebApi.Application.Models;

namespace Concepts.Crud.WebApi.Application.Commands;

public record CreateActivityCommand(ICreateActivityCommandData Data)
    : IRequest<ICollection<Activity>>;

public interface ICreateActivityCommandData : IReadOnlyCollection<IUpsertActivityCommandData>;