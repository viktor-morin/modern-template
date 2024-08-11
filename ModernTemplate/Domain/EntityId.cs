namespace ModernTemplate.Domain;

public abstract class EntityId : IComparable<EntityId>, IEquatable<EntityId>
{
    public Guid Value { get; }

    public EntityId(Guid value)
    {
        Value = value;
    }

    public bool Equals(EntityId? other) => Value.Equals(other?.Value);
    public int CompareTo(EntityId? other) => Value.CompareTo(other?.Value);

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        return obj is EntityId other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString();

    public static bool operator ==(EntityId a, EntityId b) => a.CompareTo(b) == 0;
    public static bool operator !=(EntityId a, EntityId b) => !(a == b);
}