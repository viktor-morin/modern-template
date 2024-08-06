namespace ModernTemplate.DomainModels.ValueObjects;

public sealed record Temperature
{
    private double _temperature;
    private TemperatureUnit _unit;

    public static Temperature Create(double temperature, TemperatureUnit unit)
    {
        //validation
        ArgumentNullException.ThrowIfNull(unit);


        return new Temperature
        {
            _temperature = temperature,
            _unit = unit
        };
    }

};