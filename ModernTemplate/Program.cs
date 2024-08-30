
using Asp.Versioning;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using HealthChecks.UI.Client;
using MediatR.NotificationPublishers;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using ModernTemplate;
using ModernTemplate.Database;
using ModernTemplate.Domain.UserAggregate;
using ModernTemplate.HealthCheck;
using ModernTemplate.Options;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Own
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddHostedService<OutboxHandlerService>();
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
    //dont use in singleton since they live forever, use httpclientfactory there instead
    client.BaseAddress = new Uri("");
}).AddStandardResilienceHandler();


builder.Services.AddHybridCache(); //nuget may not be needed after .net9 release


builder.Services.AddDistributedMemoryCache(); // IDistributedCache
builder.Logging.AddOpenTelemetry(config =>
{
    config.IncludeScopes = true;
    config.IncludeFormattedMessage = true;
});
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(Assembly.GetExecutingAssembly().GetName().Name!))
    .WithMetrics(options =>
    {
        options
            .AddRuntimeInstrumentation()
            .AddMeter(
                "Microsoft.AspNetCore.Hosting",
                "Microsoft.AspNetCore.Server.Kestrel",
                "System.Net.Http");
    })
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation()
            .AddNpgsql();

        tracing.AddOtlpExporter();
    })
    .UseAzureMonitor();
builder.Services.AddOptionsWithValidateOnStart<PostgresSettings>()
    .BindConfiguration("Postgres")
    .ValidateDataAnnotations();
builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>(nameof(DatabaseHealthCheck));
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddApiExplorer(options =>
  {
      options.GroupNameFormat = "'v'VVV";
      options.SubstituteApiVersionInUrl = true;
  });
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssemblies<IApplicationLayerMarker>();
    config.NotificationPublisher = new TaskWhenAllPublisher();
});

//https://www.milanjovanovic.tech/blog/response-compression-in-aspnetcore
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});


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

// correct way to line up extensionmethods, add to editorconfig file?
var apiVersionSet = app
    .NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .ReportApiVersions()
    .Build();

var versionGroup = app
    .MapGroup("api/v{version:apiVersion}")
    .WithApiVersionSet(apiVersionSet);
//MapToApiVersion on MinimalApiEndpoints

app.MapEndpoints(versionGroup);
//nest at 2024-05-31

app.UseResponseCompression();

app.UseHttpsRedirection();


//mediatr abstraction -> https://youtube.com/shorts/yuB9Ng2-2m0?si=7fIdUTnJo3BgirAm

//https://en.wikipedia.org/wiki/Single-responsibility_principle

//test naming convention -> SUT_WhenX_ShouldY
//GetMovie_WhenIdIs666_SHouldThrowException()


//todo
// folder structure image!
// check if we should use repostiory pattern, se image stored in repo
// EF + Dapper
// git rebase show
// domain events https://www.milanjovanovic.tech/blog/how-to-use-domain-events-to-build-loosely-coupled-systems?utm_source=YouTube&utm_medium=social&utm_campaign=18.03.2024
// domain events -> https://medium.com/@chaojie.xiao/domain-driven-design-practice-domain-events-15b38f3c58fc
// https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/domain-events-design-implementation
// DDD load complete aggregate root
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
// report problemDetails -> json+problem -> https://youtu.be/4NfflZilTvk?si=Q8A31zu_vauKSaFI
// opentelemtry should be implemented -> need exporter
// testname : SUT_WhenX_ShouldY -> GetMovie_WhenIdIsInvalid_ShouldReturnNotFound
// https://github.com/FortuneN/FineCodeCoverage
// use nameof instead of toString
// internal -> only in same project
// use signalr for long tasks
// DDD empty interface -> https://www.youtube.com/watch?v=6Iv57MsW-W8
// DDD roots -> https://www.youtube.com/watch?v=Pkvt87yL6Gs

// user endpoints -> https://www.youtube.com/shorts/k5VQcWE0l6g

//auth med issuer and everything -> do roles/permission as well: https://www.youtube.com/watch?v=6DWJIyipxzw

//write about double dispatch pattern -> not big fan -> https://youtu.be/wi_wsw5Gp6Q?t=206

//use sealed -> add rule
// use internal -> add rule

//open telemtry logging -> https://www.milanjovanovic.tech/blog/introduction-to-distributed-tracing-with-opentelemetry-in-dotnet

//outbox pattern -> https://www.youtube.com/watch?v=XALvnX7MPeo
// ddd -> https://www.youtube.com/watch?v=fmQJTgTdUYw
// test containers  ->  https://www.milanjovanovic.tech/blog/testcontainers-integration-testing-using-docker-in-dotnet?utm_source=YouTube&utm_medium=social&utm_campaign=04.03.2024


// Event sourcing -> https://www.youtube.com/watch?v=n_o-xuuVtmw
// log some easy events (past)
// CQRS validation, fluent -> https://www.milanjovanovic.tech/blog/cqrs-validation-with-mediatr-pipeline-and-fluentvalidation?utm_source=YouTube&utm_medium=social&utm_campaign=13.06.2024

// record for DTO's
// DDD cross-cuting stuff https://www.milanjovanovic.tech/blog/balancing-cross-cutting-concerns-in-clean-architecture?utm_source=YouTube&utm_medium=social&utm_campaign=13.05.2024

// easy auth in .net8 -> https://www.youtube.com/watch?v=S0RSsHKiD6Y

// replace guid in .net9 with newer version, make PR qbout this
//https://www.milanjovanovic.tech/blog/outbox-pattern-for-reliable-microservices-messaging?utm_source=YouTube&utm_medium=social&utm_campaign=24.06.2024

// use internal on methods inside the domain layer so etc a valueobject only can be created inside the object
// remove an object in a domain collection should use an id, not the object itself
// empty row last in file
// Add to readme.md, information about -> Clean Code, SOLID, TDD, OOP, https://en.wikipedia.org/wiki/Single-responsibility_principle
// check that we fullfill everything here https://www.youtube.com/watch?v=RfFTVwQ9oT4
// also here https://restfulapi.net/resource-naming/ -> https://www.youtube.com/watch?v=cVQEKamdLK8
// horizontal scaling -> adding nodes, vertical scaling -> adding resources
// helper class builderpatterns for testobject data -> https://www.youtube.com/watch?v=kjxf3T4tRh4
// domain events https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/domain-events-design-implementation
// Dependency Injection (DI) and Inversion of Control (IoC) are software design patterns that are used to decouple the components of an application and make it more maintainable, scalable, and testable
// TestContainer -> https://www.youtube.com/watch?v=m7r2qyUabTs

// Mention Anemic Domain Model ->  public string Name {get; set;} -> should be private set
// PDF PuppeteerSharp -> Free
// Idempotent  -> handle in services where it's important like sending email to customer (in memory cache etc)
// Ladda hela aggregate om det verkligen inte finns en performance drawback
// https://medium.com/@farkhondepeyali/enhancing-code-quality-in-net-core-with-sonaranalyzer-csharp-and-stylecop-analyzers-17e8f049d7a6
// apikey https://www.youtube.com/watch?v=CV6VdBR86co
// https://medium.com/@farkhondepeyali/enhancing-code-quality-in-net-core-with-sonaranalyzer-csharp-and-stylecop-analyzers-17e8f049d7a6
// lear to write func / action -> https://www.youtube.com/watch?v=CVQz0E33ft4

// strongly typed ID
// fine code coverage -> info

//OAUTH example  -> https://youtu.be/cgjifZF8ZME?si=iy6Q1ypJTG1N6xsT

app.MapGet("/weatherforecast", async (ClaimsPrincipal claimsPrincipal) =>
{
    await Task.Delay(1);
    var user = claimsPrincipal.Identity?.Name;
    return $"Hello {user}";
})
.WithName("GetWeatherForecast")
.RequireAuthorization()
.WithOpenApi();


static string GetGrade(int grade) =>
    grade switch
    {
        int n when n >= 90 => "A",
        int n when n >= 80 => "B",
        int n when n >= 70 => "C",
        int n when n >= 60 => "D",
        _ => "F"
    };

app.Run();


//DDD TEXT:

//07:01
//Suggested video
//MEDIATR
//JUST GOT BETTER
//m
//8:50
//Close
//Replies
//:
//We Can Finally Publish Notifications In Parall... Milan Jovanović
//Play now
//P
//@piotrkowalski3460 • 1 yr ago (edited) @MilanJovanovicTech repositories guide others on how to work with a given entity. Some examples:
//1.When you use an aggregate pattern you should access an entity only through the aggregate root. The repository pattern helps you to enforce
//it
//2. Repository can tell you whether an entity can be deleted or it abstracts a way to delete it (it's helpful with soft deletes)
//3. There are domain problems where entities can be create-only and should never be updated
//4. Repository encapsulates re-creating objects correctly from the database (helpful with eager loading related entities using.Include() etc)
//Add a reply...
//91
//X
//O

//IOC

//What does IoC stand for in C#?
//Dependency Injection (DI) and Inversion of Control (IoC) are software design patterns that are used to decouple the components of an application and make it more maintainable, scalable, and testable.Feb 4, 2023

// CQRS vs Services (domain services are okay)

//15:42
//r / csharp
//47
//Service Injections vs Mediated Command/Que... 16 upvotes. 45 comments
//chamindu · 203d
//•
//The problem with services is that they tend to get pretty big and messy over time. This violates SRP and it becomes difficult to maintain. Even if you don't use a mediator. if helps to have smaller services/handlers that do one task.
//...
//← Reply
//P
//_BigMacStack_ OP. 202d
//This is insightful. This project has the potential to bloat a normal service layer, and while CQRS creates alot of file bloat, it feels more idiomatic and will probably be easier to maintain in a controlled manner as more engineers have hands on this product.
//...
//Tenderhombre • 202d
//I've found what often leads to service bloat is cross cutting concerns. Logging, messaging, emails, notifications. You can easily end up with with a business service that needs 20 other services.
//Mediator pattern isn't always the best we handle these. Look at events, decorator

//test brokeers domain events 
// https://www.milanjovanovic.tech/blog/testing-modular-monoliths-system-integration-testing

//Lock for unique email -> application layer
// if you have complicated and everchanging buisness rules,
// involving validation, calculations, and derivations,
// chances are that you'll want an object model to handle them
// Domain Model - PoEAA

//catch (DbUpdateException e)
// when (e.InnerException is NpgsqlException { SqlState: PostgresErrorCodes.UniqueViolation })
// {}