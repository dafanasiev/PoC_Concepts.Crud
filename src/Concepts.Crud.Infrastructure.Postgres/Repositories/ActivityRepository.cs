using Concepts.Crud.Domain.AggregatesModel.ActivityAggregate;
using Concepts.Crud.Domain.SeedWork;
using Activity = Concepts.Crud.Domain.AggregatesModel.ActivityAggregate.Activity;

namespace Concepts.Crud.Infrastructure.Postgres.Repositories;

public class ActivityRepository
    : IActivityRepository
{
    private readonly ICrudContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public ActivityRepository(ICrudContext context)
    {
        _context = context ?? throw new ArgumentNullException();
    }

    public async Task<ICollection<Activity>> AddRange(ICollection<Activity> items, CancellationToken ct)
    {
        await _context.ActivitySet.AddRange(items, ct);
        return items;
    }

    public Task<Activity?> Find(Guid id, CancellationToken ct)
    {
        return _context.ActivitySet
            .Include(x=>x.RelationshipList)
            .FirstOrDefaultAsync(x => x.Id == id && x.GC == null && x.GC == null, ct);
    }

    // public Task Update(Activity activity, CancellationToken ct)
    // {
    //     return _context.ActivitySet.Update(activity, ct);
    // }
}