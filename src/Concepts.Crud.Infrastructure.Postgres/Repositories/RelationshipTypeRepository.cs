using Concepts.Crud.Domain.AggregatesModel.RelationshipTypeAggregate;
using Concepts.Crud.Domain.SeedWork;

namespace Concepts.Crud.Infrastructure.Postgres.Repositories;

public class RelationshipTypeRepository
:IRelationshipTypeRepository
{
    private readonly ICrudContext _context;
    public IUnitOfWork UnitOfWork => _context;
    public RelationshipTypeRepository(ICrudContext context)
    {
        _context = context ?? throw new ArgumentNullException();
    }

    public async Task<RelationshipType?> FindById(Guid id, CancellationToken ct)
    {
        var rv = await _context.RelationshipTypeSet.FirstOrDefaultAsync(x=>x.Id == id && x.GC == null, ct);
        return rv;
    }
}