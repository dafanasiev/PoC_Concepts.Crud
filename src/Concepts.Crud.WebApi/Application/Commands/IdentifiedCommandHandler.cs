using Concepts.Crud.Infrastructure.Idempotency;
using Concepts.Crud.WebApi.Extensions;

namespace Concepts.Crud.WebApi.Application.Commands;

public abstract class IdentifiedCommandHandler<T, R, TData> : IRequestHandler<IdentifiedCommand<T, R>, R>
    where T : IRequest<R>
{
    private readonly IMediator _mediator;
    protected readonly IRequestManager RequestManager;
    protected readonly ILogger<IdentifiedCommandHandler<T, R, TData>> Logger;

    public IdentifiedCommandHandler(
        IMediator mediator,
        IRequestManager requestManager,
        ILogger<IdentifiedCommandHandler<T, R, TData>> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        _mediator = mediator;
        RequestManager = requestManager;
        Logger = logger;
    }

    /// <summary>
    /// Creates the result value to return if a previous request was found
    /// </summary>
    /// <returns></returns>
    protected abstract Task<R> CreateResultForDuplicateRequest(TData data, CancellationToken ct);

    /// <summary>
    /// This method handles the command. It just ensures that no other request exists with the same ID, and if this is the case
    /// just enqueues the original inner command.
    /// </summary>
    /// <param name="message">IdentifiedCommand which contains both original command & request ID</param>
    /// <returns>Return value of inner command or default value if request same ID was found</returns>
    public virtual async Task<R> Handle(IdentifiedCommand<T, R> message, CancellationToken ct)
    {
        var data = default(TData);
        var alreadyExists = await RequestManager.ExistAsync<TData>(
            message.Id,
            typeof(TData) == typeof(bool) ? null : (d) => data = d,
            ct);
        if (alreadyExists)
        {
            return await CreateResultForDuplicateRequest(data!, ct);
        }

        await RequestManager.CreateRequestForCommandAsync<T>(message.Id, ct);

        // TODO: more vars for logging ?
        var command = message.Command;
        var commandName = command.GetGenericTypeName();

        try
        {
            switch (command)
            {
                case CreateActivityCommand _:
                    break;

                default:
                    break;
            }

            Logger.LogInformation(
                "Sending command: {CommandName} - {@Command}",
                commandName,
                command);

            // Send the embedded business command to mediator so it runs its related CommandHandler 
            var result = await _mediator.Send(command, ct);

            Logger.LogInformation(
                "Command result: {@Result} - {CommandName} - {@Command}",
                result,
                commandName,
                command);

            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(
                ex,
                "Command execution error: {CommandName} - {@Command}",
                commandName,
                command
            );
            throw;
        }
    }
}