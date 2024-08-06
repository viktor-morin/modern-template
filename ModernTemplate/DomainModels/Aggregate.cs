using System.Diagnostics.CodeAnalysis;

namespace ModernTemplate.DomainModels;

[ExcludeFromCodeCoverage]
public abstract class Aggregate
{
    public abstract AggregateId Id { get; init; }
    public DateTime Created { get; init; } = DateTime.UtcNow;
    public DateTime Updated { get; private set; } = DateTime.UtcNow;

    protected void UpdateAggregateTimestamp()
    {
        Updated = DateTime.UtcNow;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (obj is not AggregateId entity)
            return false;

        return Id.Value.Equals(entity.Value) == true;
    }

    public override int GetHashCode()
    {
        return Id.Value.GetHashCode();
    }

    public static bool operator ==(Aggregate a, Aggregate b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(Aggregate a, Aggregate b)
    {
        return !(a == b);
    }
}