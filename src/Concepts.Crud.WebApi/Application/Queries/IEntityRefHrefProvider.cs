using Concepts.Crud.WebApi.Application.Models;

namespace Concepts.Crud.WebApi.Application.Queries;

public interface IEntityRefHrefProvider
{
    Task<Uri> Obtain(EntityRef eRef, CancellationToken ct);
}