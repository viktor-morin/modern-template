
using Asp.Versioning;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using ModernTemplate;
using ModernTemplate.Database;
using ModernTemplate.DomainModels.Aggregates;
using ModernTemplate.HealthCheck;
using ModernTemplate.Options;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using System.Reflection;
using System.Runtime.CompilerServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Own
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddHostedService<OutboxService>();
builder.Services.Configure<HostOptions>(options =>
{
    options.ServicesStartConcurrently = true;
    options.ServicesStopConcurrently = true;
});
builder.Services.AddAuthorization();
builder.Services
    .AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddCookie(IdentityConstants.ApplicationScheme)
    .AddBearerToken(IdentityConstants.BearerScheme);
builder.Services.AddIdentityCore<User>() //AddEntityFrameworkSTores (context) ->  but we should use dapper for query
    .AddApiEndpoints();
builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());
builder.Services.AddHttpClient<HttpService>(client =>
{
    client.BaseAddress = new Uri("");
}); //dont use in singleton since they live forever, use httpclientfactory there instead
builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache(); // IDistributedCache
builder.Logging.AddOpenTelemetry(config =>
{
    config.IncludeScopes = true;
    config.IncludeFormattedMessage = true;
});
builder.Services.AddOpenTelemetry()
    .WithMetrics(options =>
    {
        options
            .AddRuntimeInstrumentation()
            .AddMeter(
                "Microsoft.AspNetCore.Hosting",
                "Microsoft.AspNetCore.Server.Kestrel",
                "System.Net.Http");
    })
    .WithTracing(options =>
    {
        options
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();
    });
builder.Services.AddOptionsWithValidateOnStart<PostgresSettings>()
    .BindConfiguration("Postgres")
    .ValidateDataAnnotations();
builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>(nameof(DatabaseHealthCheck));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// Own
app.UseExceptionHandler();
app.MapHealthChecks("/health",
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
app.MapIdentityApi<User>();

var apiVersionSet = app
    .NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .ReportApiVersions()
    .Build();

var versionGroup = app
    .MapGroup("api/v{version:apiVersion}")
    .WithApiVersionSet(apiVersionSet);

app.MapEndpoints(versionGroup);
//nest at 2024-04-21


app.UseHttpsRedirection();


//todo
// Write about <WarningsAsErrors>CS8602</WarningsAsErrors> and set it on all projects
// Write about explicit declarate references such as -> "UserService userService = new UserService()" instead of "var userService = new UserService()" where it makes sense
// PDF generation Quest PDF -> // ironPDF (quest gratis?)
// maybe serilog, lets see later? https://www.youtube.com/watch?v=w7yDuoCLVvQ
// show -> if (context.User.Identity is not { IsAuthenitcated: true })
// NetArchTest.Rules
// https://andrewlock.net/using-strongly-typed-entity-ids-to-avoid-primitive-obsession-part-1/
// https://www.milanjovanovic.tech/blog/cqrs-validation-with-mediatr-pipeline-and-fluentvalidation
// test containers https://www.milanjovanovic.tech/blog/testcontainers-integration-testing-using-docker-in-dotnet?utm_source=YouTube&utm_medium=social&utm_campaign=04.03.2024
// where T is : class, IEvent
// report problemDetails -> json+problem
// opentelemtry should be implemented -> need exporter
// testname : SUT_WhenX_ShouldY -> GetMovie_WhenIdIsInvalid_ShouldReturnNotFound



var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};




app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.RequireAuthorization()
.WithOpenApi();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}