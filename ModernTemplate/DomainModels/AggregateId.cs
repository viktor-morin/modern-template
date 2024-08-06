namespace ModernTemplate.DomainModels;

public abstract class AggregateId : IComparable<AggregateId>, IEquatable<AggregateId>
{
    public Guid Value { get; }

    public AggregateId(Guid value)
    {
        Value = value;
    }

    public bool Equals(AggregateId? other) => Value.Equals(other?.Value);
    public int CompareTo(AggregateId? other) => Value.CompareTo(other?.Value);

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        return obj is AggregateId other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString();

    public static bool operator ==(AggregateId a, AggregateId b) => a.CompareTo(b) == 0;
    public static bool operator !=(AggregateId a, AggregateId b) => !(a == b);
}