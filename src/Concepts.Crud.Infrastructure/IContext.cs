namespace Concepts.Crud.Infrastructure;

public interface IContext
{
    bool HasActiveTransaction { get;  }
    Task<ITransaction?> BeginTransactionAsync();
    Task CommitTransactionAsync(ITransaction transaction);
    Task ExecuteAsync(Func<Task> action);
}