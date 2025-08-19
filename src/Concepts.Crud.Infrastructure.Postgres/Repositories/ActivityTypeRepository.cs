using Concepts.Crud.Domain.AggregatesModel.ActivityTypeAggregate;
using Concepts.Crud.Domain.SeedWork;

namespace Concepts.Crud.Infrastructure.Postgres.Repositories;

public class ActivityTypeRepository
:IActivityTypeRepository
{
    private readonly ICrudContext _context;
    public IUnitOfWork UnitOfWork => _context;
    public ActivityTypeRepository(ICrudContext context)
    {
        _context = context ?? throw new ArgumentNullException();
    }

    public async Task<ActivityType?> FindById(Guid id, CancellationToken ct)
    {
        var rv = await _context.ActivityTypeSet.FirstOrDefaultAsync(x => x.Id == id && x.GC ==null, ct);
        return rv;
    }
}