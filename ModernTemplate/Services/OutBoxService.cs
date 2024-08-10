using ModernTemplate.Database;
using ModernTemplate.DomainModels.DomainEvents;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace ModernTemplate.Services;

public sealed class OutBoxService
{
    private readonly IApplicationDbContext _dbContext;

    public OutBoxService(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddDomainEvent(IDomainEvent domainEvent)
    {
        await AddDomainEvents([domainEvent]);
    }

    public async Task AddDomainEvents(IEnumerable<IDomainEvent> domainEvents)
    {
        var outboxMessages = domainEvents.Select(domainEvent => new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Content = JsonSerializer.Serialize(
                domainEvent,
                options: new JsonSerializerOptions
                {
                    typ
                }),
            CreatedAtUtc = DateTime.UtcNow,
            Type = domainEvent.GetType().Name,
            Data = JsonSerializer.Serialize(domainEvent),
            Processed = false
        });

        //how to deserialize since it's derivate from a base class....


        var outboxMEssage = new OutboxMessage().ToString();

        var test = JsonSerializer.Deserialize(outboxMEssage, );
        await _dbContext.SaveChangesAsync();
    }

    public async Task HandleDomainEvents()
    {
        var domainEvents = _dbContext.DomainEvents.ToList();

    }
}


public class test : IDomainEventHandler