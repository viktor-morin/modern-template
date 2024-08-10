using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
using ModernTemplate.DomainModels;
using ModernTemplate.DomainModels.Aggregates;
using ModernTemplate.Options;

namespace ModernTemplate.Database;

public sealed class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private string _connectionString { get; init; }
    public DbSet<Weather> Weathers { get; init; }

    private readonly IPublisher _publisher;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IPublisher publisher,
        IOptions<PostgresSettings> settings) : base(options)
    {
        _publisher = publisher;
        _connectionString = settings.Value.ConnectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);

        await PublishDomainEventsAsync();

        return result;
    }

    //https://www.milanjovanovic.tech/blog/how-to-use-domain-events-to-build-loosely-coupled-systems?utm_source=YouTube&utm_medium=social&utm_campaign=29.04.2024
    //https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/domain-events-design-implementation
    private async Task PublishDomainEventsAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity)
            .SelectMany(entites =>
            {
                var domainEvents = entites.GetDomainEvents();
                entites.ClearDomainEvents();
                return domainEvents;
            });


        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }
    }
}

public interface IApplicationDbContext
{
    public DbSet<Weather> Weathers { get;  }
    public DatabaseFacade Database { get; }
}