namespace Concepts.Crud.Infrastructure;

public interface IMutableQueryable<T>
:IQueryable<T>
{
    Task AddRange(IEnumerable<T> items, CancellationToken ct);
    Task Add(T item, CancellationToken ct);
}