using System.Collections;
using System.Linq.Expressions;
using Concepts.Crud.Domain.AggregatesModel.ActivityAggregate;
using Concepts.Crud.Domain.AggregatesModel.ActivityRelationshipAggregate;
using Concepts.Crud.Domain.AggregatesModel.ActivityTypeAggregate;
using Concepts.Crud.Domain.AggregatesModel.ClientRequestAggregate;
using Concepts.Crud.Domain.AggregatesModel.EntityRefAggregate;
using Concepts.Crud.Domain.AggregatesModel.RelationshipTypeAggregate;
using Concepts.Crud.Domain.SeedWork;
using MediatR;
#if false
using Concepts.Crud.Domain.AggregatesModel.ActivityGraphAggregate;
#endif

namespace Concepts.Crud.Infrastructure.Postgres;

public class CrudContext
    : BaseContext, ICrudContext
{
    private DbSetWap<Activity>? _activitySet;
    public DbSet<ActivityType> ActivityTypeSet { get; set; }

    IQueryable<ActivityType> ICrudContext.ActivityTypeSet => ActivityTypeSet;

    public DbSet<RelationshipType> RelationshipTypeSet { get; set; }
    IQueryable<RelationshipType> ICrudContext.RelationshipTypeSet => RelationshipTypeSet;
    public DbSet<Activity> ActivitySet { get; set; }
    IMutableQueryable<Activity> ICrudContext.ActivitySet => _activitySet ??= new ActivitySetDbWrap(ActivitySet);

    private DbSetWap<ClientRequest>? _clientRequestSet;
    public DbSet<ClientRequest> ClientRequestSet { get; set; }
    IMutableQueryable<ClientRequest> ICrudContext.ClientRequestSet => _clientRequestSet ??= new ClientRequestSetDbWrap(ClientRequestSet);

    public DbSet<EntityRef> EntityRefSet { get; set; }
    IQueryable<EntityRef> ICrudContext.EntityRefSet => EntityRefSet;

    public DbSet<ActivityRelationship> ActivityRelationshipSet { get; set; }
    IQueryable<ActivityRelationship> ICrudContext.ActivityRelationshipSet => ActivityRelationshipSet;

#if false
    public DbSet<ActivityGraph> ActivityGraphSet { get; set; }
    IQueryable<ActivityGraph> ICrudContext.ActivityGraphSet => ActivityGraphSet;
#endif

    internal CrudContext(DbContextOptions<CrudContext> options) : base(options)
    {
    }

    public CrudContext(DbContextOptions<CrudContext> options, IMediator mediator) : base(options, mediator)
    {
        System.Diagnostics.Debug.WriteLine("CrudContext::ctor ->" + GetHashCode());
    }

    abstract class DbSetWap<T>(DbSet<T> dbSet) : IMutableQueryable<T>
        where T : SimpleEntity<Guid>
    {
        protected readonly DbSet<T> dbSet = dbSet;

        public Task AddRange(IEnumerable<T> items, CancellationToken ct)
        {
            return dbSet.AddRangeAsync(items, ct);
        }

        public async Task Add(T item, CancellationToken ct)
        {
            _ = await dbSet.AddAsync(item, ct);
        }
        
        public IEnumerator<T> GetEnumerator()
        {
            return ((IQueryable<T>) dbSet).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Type ElementType => ((IQueryable<T>) dbSet).ElementType;
        public Expression Expression => ((IQueryable<T>) dbSet).Expression;
        public IQueryProvider Provider => ((IQueryable<T>) dbSet).Provider;
    }

    class ClientRequestSetDbWrap(DbSet<ClientRequest> dbSet) : DbSetWap<ClientRequest>(dbSet);
    class ActivitySetDbWrap(DbSet<Activity> dbSet) : DbSetWap<Activity>(dbSet);
}