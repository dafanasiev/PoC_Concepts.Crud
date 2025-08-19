namespace Concepts.Crud.WebApi.Api;

public interface IProblemHrefProvider
{
    string GetExecuteCommandProblemRef(string commandName, Guid commandId);
}