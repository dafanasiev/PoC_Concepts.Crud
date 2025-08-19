using Concepts.Crud.Domain.AggregatesModel.EntityRefAggregate;
using Concepts.Crud.Domain.SeedWork;

namespace Concepts.Crud.Infrastructure.Postgres.Repositories;

public class EntityRefRepository
:IEntityRefRepository
{
    private readonly ICrudContext _context;
    public IUnitOfWork UnitOfWork => _context;
    public EntityRefRepository(ICrudContext context)
    {
        _context = context ?? throw new ArgumentNullException();
    }

    public async Task<EntityRef?> FindById(Guid id, CancellationToken ct)
    {
        var rv = await _context.EntityRefSet.FirstOrDefaultAsync(x=>x.Id == id && x.GC == null, ct);
        return rv;
    }
}