
namespace ModernTemplate.DomainModels.Ids;

//todo
//https://andrewlock.net/using-strongly-typed-entity-ids-to-avoid-primitive-obsession-part-1/
public sealed class WeatherId : AggregateId
{
    public WeatherId() : base(Guid.NewGuid()) { }
}