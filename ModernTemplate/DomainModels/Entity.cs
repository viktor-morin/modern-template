using ModernTemplate.DomainModels.DomainEvents;

namespace ModernTemplate.DomainModels;

public abstract class Entity
{
    public abstract EntityId Id { get; init; }
    public DateTime Created { get; init; } = DateTime.UtcNow;
    public DateTime Updated { get; private set; } = DateTime.UtcNow;

    private readonly List<IDomainEvent> _domainEvents = new();

    protected void UpdateAggregateTimestamp()
    {
        Updated = DateTime.UtcNow;
    }

    public IEnumerable<IDomainEvent> GetDomainEvents()
    {
        return _domainEvents;
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void RaiseDomainEvents(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (obj is not EntityId entity)
            return false;

        return Id.Value.Equals(entity.Value) == true;
    }

    public override int GetHashCode()
    {
        return Id.Value.GetHashCode();
    }

    public static bool operator ==(Entity a, Entity b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(Entity a, Entity b)
    {
        return !(a == b);
    }
}