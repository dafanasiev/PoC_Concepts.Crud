using Concepts.Crud.WebApi.Application.Commands;

namespace Concepts.Crud.WebApi.Api.Activity;

public class CreateActivityRequest
    : List<UpsertActivityRequest>,
        ICreateActivityCommandData
{
    public IEnumerator<IUpsertActivityCommandData> GetEnumerator()
    {
        return base.GetEnumerator();
    }
}