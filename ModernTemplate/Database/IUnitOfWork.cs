using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ModernTemplate.DomainModels.Aggregates;

namespace ModernTemplate.Database;

public interface IUnitOfWork
{
    //https://en.wikipedia.org/wiki/Unit_of_work#:~:text=A%20unit%20of%20work%20is,a%20result%20of%20the%20work.
    public DbSet<Weather> Weathers { get; }
    public DbSet<OutboxMessage> OutboxMessages { get; init; }
    public DatabaseFacade Database { get; }
}