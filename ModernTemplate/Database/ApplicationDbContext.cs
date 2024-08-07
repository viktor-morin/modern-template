using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
using ModernTemplate.DomainModels.Aggregates;
using ModernTemplate.Options;

namespace ModernTemplate.Database;

public sealed class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private string _connectionString { get; init; }
    public DbSet<Weather> Weathers { get; init; }

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
}

public interface IApplicationDbContext
{
    public DbSet<Weather> Weathers { get;  }
    public DatabaseFacade Database { get; }
}