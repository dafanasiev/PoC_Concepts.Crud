namespace Concepts.Crud.Infrastructure;

public interface ITransaction : IAsyncDisposable
{
    Guid TransactionId { get;  }
}