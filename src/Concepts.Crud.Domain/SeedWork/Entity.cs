namespace Concepts.Crud.Domain.SeedWork;

public abstract class Entity
{
    private List<INotification>? _domainEvents;
    public IReadOnlyCollection<INotification>? DomainEvents => _domainEvents?.AsReadOnly();

    public void AddDomainEvent(INotification eventItem)
    {
        _domainEvents ??= [];
        _domainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(INotification eventItem)
    {
        _domainEvents?.Remove(eventItem);
    }

    public void ClearDomainEvents()
    {
        _domainEvents?.Clear();
    }
}

public abstract class Entity<TId> : SimpleEntity<TId>
    where TId : IEquatable<TId>
{
    /// <summary>
    /// int>0 indicates that record was deleted (deferred deletion)
    /// </summary>
    public uint? GC { get; set; }

    public virtual void Delete()
    {
        GC = (uint) DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    public override bool Equals(object? obj)
    {
        if (base.Equals(obj))
        {
            return (((Entity<TId>) obj).GC == null && GC == null)
                   || (((Entity<TId>) obj).GC != null && GC != null);
        }
        return false;
    }
}

public abstract class SimpleEntity<TId> : Entity
    where TId : IEquatable<TId>
{
    int? _requestedHashCode;
    TId? _Id;

    public virtual TId Id
    {
        get => _Id ?? throw new Exception("Id cannot be null");
        protected set => _Id = value;
    }

    public bool IsTransient()
    {
        return Id.Equals(default);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(obj, null) || !(obj is Entity<TId>))
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        if (GetType() != obj.GetType())
            return false;

        var item = (Entity<TId>) obj;

        if (item.IsTransient() || IsTransient())
            return false;
        
        return item.Id.Equals(Id);
    }

    public override int GetHashCode()
    {
        if (!IsTransient())
        {
            if (!_requestedHashCode.HasValue)
                _requestedHashCode = Id.GetHashCode() ^ 31; // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)

            return _requestedHashCode.Value;
        }

        return base.GetHashCode();
    }

    public static bool operator ==(SimpleEntity<TId>? left, SimpleEntity<TId>? right)
    {
        if (ReferenceEquals(left, null))
            return ReferenceEquals(right, null);

        return left.Equals(right);
    }

    public static bool operator !=(SimpleEntity<TId>? left, SimpleEntity<TId>? right)
    {
        return !(left == right);
    }
}