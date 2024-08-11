using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ModernTemplate.Domain;
using ModernTemplate.DomainModels.Aggregates;
using ModernTemplate.Options;
using System.Text.Json;

namespace ModernTemplate.Database;

public sealed class ApplicationDbContext : DbContext, IUnitOfWork
{
    private string _connectionString { get; init; }
    public DbSet<Weather> Weathers { get; init; }
    public DbSet<OutboxMessage> OutboxMessages { get; init; }

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IOptions<PostgresSettings> settings) : base(options)
    {
        _connectionString = settings.Value.ConnectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        PersistDomainEvents();

        return await base.SaveChangesAsync(cancellationToken);
    }

    //https://www.milanjovanovic.tech/blog/how-to-use-domain-events-to-build-loosely-coupled-systems?utm_source=YouTube&utm_medium=social&utm_campaign=29.04.2024
    //https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/domain-events-design-implementation
    private void PersistDomainEvents()
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

        var outboxMessages = domainEvents
            .Select(domainEvent => new OutboxMessage
            {
                Content = JsonSerializer.Serialize(domainEvent),
                Type = domainEvent.GetType().Name,
            });

        OutboxMessages.AddRange(outboxMessages);
    }
}