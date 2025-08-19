using Microsoft.EntityFrameworkCore.Storage;

namespace Concepts.Crud.Infrastructure.Postgres;

public class Transaction(IDbContextTransaction impl)
    : ITransaction
{
    public ValueTask DisposeAsync()
    {
        return impl.DisposeAsync();
    }

    public IDbContextTransaction Tx => impl;
    public Guid TransactionId => impl.TransactionId;
}