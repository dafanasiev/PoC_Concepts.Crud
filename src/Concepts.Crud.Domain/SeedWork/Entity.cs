namespace Concepts.Crud.Domain.SeedWork;

public abstract class SimpleEntity
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

    public abstract bool IsTransient();

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return true;

        if (obj is not SimpleEntity other)
            return false;

        if (other.IsTransient() || IsTransient())
            return false;

        return true;
    }

    public static bool operator ==(SimpleEntity? left, SimpleEntity? right)
    {
        if (ReferenceEquals(left, null))
            return ReferenceEquals(right, null);

        return left.Equals(right);
    }

    public static bool operator !=(SimpleEntity? left, SimpleEntity? right)
    {
        return !(left == right);
    }
}

public abstract class Entity : SimpleEntity
{
    /// <summary>
    /// int>0 indicates that record was deleted (deferred deletion)
    /// </summary>
    public uint? GC { get; private set; }

    public override bool Equals(object? obj)
    {
        if (base.Equals(obj))
        {
            if (obj is not Entity other)
                return false;

            return (other.GC, GC) is (null, null) or (not null, not null);
        }

        return false;
    }

    public virtual void Delete()
    {
        GC = (uint) DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}

public abstract class SimpleEntity<TId> : SimpleEntity
    where TId : IEquatable<TId>
{
    int? _requestedHashCode;
    TId? _Id;

    public TId Id
    {
        get => _Id ?? throw new Exception("Id cannot be null");
        protected set => _Id = value;
    }

    public override bool Equals(object? obj)
    {
        if (base.Equals(obj))
        {
            if (obj is not SimpleEntity<TId> other)
                return false;

            if (other.IsTransient() || IsTransient())
                return false;

            return other.Id.Equals(Id);
        }

        return false;
    }

    public override int GetHashCode()
    {
        if (!IsTransient())
        {
            if (!_requestedHashCode.HasValue)
                _requestedHashCode =
                    Id.GetHashCode() ^
                    31; // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)

            return _requestedHashCode.Value;
        }

        return base.GetHashCode();
    }


    public override bool IsTransient()
    {
        return Id.Equals(default);
    }
}

public abstract class Entity<TId> : Entity
    where TId : IEquatable<TId>
{
    int? _requestedHashCode;
    TId? _Id;

    public TId Id
    {
        get => _Id ?? throw new Exception("Id cannot be null");
        protected set => _Id = value;
    }

    public override bool Equals(object? obj)
    {
        if (base.Equals(obj))
        {
            if (obj is not Entity<TId> other)
                return false;

            if (other.IsTransient() || IsTransient())
                return false;

            return other.Id.Equals(Id);
        }

        return false;
    }

    public override int GetHashCode()
    {
        if (!IsTransient())
        {
            if (!_requestedHashCode.HasValue)
                _requestedHashCode =
                    Id.GetHashCode() ^
                    31; // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)

            return _requestedHashCode.Value;
        }

        return base.GetHashCode();
    }


    public override bool IsTransient()
    {
        return Id.Equals(default);
    }
}