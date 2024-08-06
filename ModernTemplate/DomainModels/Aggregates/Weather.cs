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
}