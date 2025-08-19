using System.Data;
using Concepts.Crud.Domain.SeedWork;
using Concepts.Crud.Infrastructure.Postgres.EntityConfigurations;
using MediatR;

namespace Concepts.Crud.Infrastructure.Postgres;

public abstract class BaseContext
    : DbContext, IUnitOfWork
{
    // ReSharper disable once MemberCanBePrivate.Global
    protected Transaction? CurrentTransaction;
    protected readonly IMediator Mediator;

    internal BaseContext(DbContextOptions<CrudContext> options) : base(options)
    {
        // this ctor only for internal usage (db migrations)
        Mediator = null!;
    }

    protected BaseContext(DbContextOptions<CrudContext> options, IMediator mediator) : base(options)
    {
        Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public bool HasActiveTransaction => CurrentTransaction != null;

    public async Task<ITransaction?> BeginTransactionAsync()
    {
        if (CurrentTransaction != null) return null;
        var tx = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        CurrentTransaction = new(tx);
        return CurrentTransaction;
    }

    public async Task CommitTransactionAsync(ITransaction transaction)
    {
        if (transaction == null) throw new ArgumentNullException(nameof(transaction));
        var iTransaction = (Transaction) transaction;
        if (!ReferenceEquals(transaction, CurrentTransaction)) throw new InvalidOperationException($"Transaction {iTransaction.Tx.TransactionId} is not current");

        try
        {
            await SaveChangesAsync();
            await iTransaction.Tx.CommitAsync();
        }
        catch
        {
            await RollbackTransaction();
            throw;
        }
        finally
        {
            if (CurrentTransaction != null)
            {
                await CurrentTransaction.DisposeAsync();
                CurrentTransaction = null;
            }
        }
    }

    public Task ExecuteAsync(Func<Task> action)
    {
        var strategy = Database.CreateExecutionStrategy();
        return strategy.ExecuteAsync(action);
    }

    private async Task RollbackTransaction()
    {
        try
        {
            await CurrentTransaction?.Tx.RollbackAsync()!;
        }
        finally
        {
            if (CurrentTransaction != null)
            {
                await CurrentTransaction.DisposeAsync();
                CurrentTransaction = null;
            }
        }
    }


    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        // Dispatch Domain Events collection. 
        // Choices:
        // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
        // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
        // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
        // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
        await Mediator.DispatchDomainEventsAsync(this);

        // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
        // performed through the DbContext will be committed
        _ = await base.SaveChangesAsync(cancellationToken);

        return true;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("crud");
        //TODO: collation support modelBuilder.HasCollation("case_insensitive", locale: "ru_RU-u-ks-level2", provider: "icu", deterministic: false);
        modelBuilder.ApplyConfiguration(new ActivityEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ActivityRelationshipEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ActivityTypeEntityConfiguration());
        modelBuilder.ApplyConfiguration(new EntityRefEntityConfiguration());
#if false
        modelBuilder.ApplyConfiguration(new ActivityGraphConfiguration());
#endif
        modelBuilder.ApplyConfiguration(new RelationshipTypeEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ClientRequestEntityConfiguration());

        //TODO? modelBuilder.UseIntegrationEventLogs();
    }
}