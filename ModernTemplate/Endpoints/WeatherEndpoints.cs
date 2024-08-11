using MediatR;

namespace ModernTemplate.Endpoints;

public class WeatherEndpoints : IEndpoint
{
    private const string TAG = "Weather";

    public void MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet(
            "weather/{city}", async (
                string city,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                await Task.Delay(1);
                return Results.Ok();
            })
            .WithOpenApi()
            .MapToApiVersion(1)
            .WithTags(TAG);
    }
}