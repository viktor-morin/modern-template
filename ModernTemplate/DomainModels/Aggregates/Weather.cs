using ModernTemplate.DomainModels.DomainEvents;
using ModernTemplate.DomainModels.Ids;
using ModernTemplate.DomainModels.ValueObjects;

namespace ModernTemplate.DomainModels.Aggregates;

public sealed class Weather : Aggregate
{
    private Temperature _temperature;

    private Weather(Temperature temperature)
    {
        _temperature = temperature;
    }

    public override AggregateId Id { get; init; } = new WeatherId();

    public static Weather Create(double temperature, TemperatureUnit unit)
    {

        // todo
        // validation

        var w = new Weather(Temperature.Create(10, TemperatureUnit.Celsius));


        return new Weather(Temperature.Create(temperature, unit));
    }

    public void TempToShowEvent()
    {
        RaiseDomainEvents(TempEvent.)
    }
}


public sealed class TempEvent : IDomainEvent
{
    public WeatherId WeatherId { get; init; }

    private TempEvent(WeatherId weatherId)
    {
        WeatherId = weatherId;
    }  
    
    public static TempEvent Create(WeatherId weatherId)
    {
        ArgumentNullException.ThrowIfNull(weatherId);

        return new TempEvent(weatherId);
    }
}