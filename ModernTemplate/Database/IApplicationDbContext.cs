using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ModernTemplate.Domain.UserAggregate;
using ModernTemplate.Domain.WorkoutAggregate;

namespace ModernTemplate.Database;

public interface IApplicationDbContext
{
    public DatabaseFacade Database { get; }

    public DbSet<User> Users { get; init; }
    public DbSet<Workout> Workouts { get; init; }

    public DbSet<OutboxMessage> OutboxMessages { get; init; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}