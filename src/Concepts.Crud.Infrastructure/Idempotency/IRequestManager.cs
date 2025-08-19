namespace Concepts.Crud.Infrastructure.Idempotency;

public interface IRequestManager
{
    Task<bool> ExistAsync<TData>(Guid requestId, Action<TData>? dataCb, CancellationToken ct);

    Task CreateRequestForCommandAsync<T>(Guid requestId, CancellationToken ct);
    
    Task AcknowledgeAsync<TData>(Guid requestId, TData data, CancellationToken ct);
}