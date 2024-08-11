using Microsoft.AspNetCore.Http.HttpResults;
using ModernTemplate.Database;
using ModernTemplate.Domain.WorkoutAggregate;
using System.Diagnostics;

namespace ModernTemplate.VerticalSlice.Features.CreateActivity;

internal static class CreateWorkout
{
    private const string TAG = "Workout";

    internal record Request(string Name, int NumberOfExcercises);
    internal record Response(Guid Id, string Name, int NumberOfExcercises);

    internal class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder
                .MapPost("/workout", Handler)
                .WithOpenApi()
                .WithTags(TAG);
        }

        public static async Task<Ok<Response>> Handler(
            Request request, 
            IApplicationDbContext dbContext,
            CancellationToken cancellationToken)
        {
            var workout = Workout.Create(request.Name, request.NumberOfExcercises);

            dbContext.Workouts.Add(workout);

            Debugger.Break();
            //verify cancellationToken is there

            await dbContext.SaveChangesAsync(cancellationToken);

            return TypedResults.Ok(new Response(
                workout.Id.Value, 
                workout.Name,
                workout.NumberOfExcercises));
        }
    }
}
