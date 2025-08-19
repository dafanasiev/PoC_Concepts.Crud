using System.Diagnostics;
using System.Text.Json;
using Concepts.Crud.Domain.AggregatesModel.ClientRequestAggregate;
using Concepts.Crud.Domain.Exceptions;
using Concepts.Crud.Infrastructure.Idempotency;

namespace Concepts.Crud.Infrastructure.Postgres.Idempotency;

public class RequestManager(ICrudContext context) : IRequestManager
{
    public async Task<bool> ExistAsync<TData>(Guid requestId, Action<TData>? dataCb, CancellationToken ct)
    {
        if (typeof(TData) == typeof(bool))
        {
            var rv = await context.ClientRequestSet
                .AsNoTracking()
                .AnyAsync(x => x.Id == requestId, ct);

            if (dataCb is null)
            {
                return rv;
            }

            Debug.Assert(dataCb == null);
            throw new ArgumentException(
                "do not use callback for bool - just use result of method call",
                nameof(dataCb)
            );
        }
        else
        {
            var rv = await context.ClientRequestSet
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == requestId, ct);

            if (dataCb is null)
            {
                return rv == null;
            }

            if (rv?.Data is null)
            {
                dataCb(default!);
                return false;
            }

            var data = JsonSerializer.Deserialize<TData>(rv.Data) ?? default(TData);
            dataCb(data!);
            return true;
        }
    }

    public async Task CreateRequestForCommandAsync<T>(Guid requestId, CancellationToken ct)
    {
        var exists = await ExistAsync<bool>(requestId, null, ct);

        var request = exists
                ? throw new CrudDomainException($"Request with {requestId} already exists")
                : new ClientRequest(
                    requestId,
                    typeof(T).Name,
                    DateTime.UtcNow,
                    null
                )
            ;

        await context.ClientRequestSet.Add(request, ct);

        await context.SaveChangesAsync(ct);
    }

    public async Task AcknowledgeAsync<TData>(Guid requestId, TData data, CancellationToken ct)
    {
        var e = await context.ClientRequestSet.FirstOrDefaultAsync(x=>x.Id == requestId, ct);
        if (e is null)
        {
            throw new CrudDomainException($"Request with {requestId} does not exist");
        }
        e.SetData(data == null ? null : JsonSerializer.Serialize(data));
        //await context.SaveChangesAsync(ct); //TODO: checkme
    }
}