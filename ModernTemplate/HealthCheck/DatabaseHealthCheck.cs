using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ModernTemplate.Database;

namespace ModernTemplate.HealthCheck;

public sealed class DatabaseHealthCheck : IHealthCheck
{
    private readonly IUnitOfWork _dbContext;

    public DatabaseHealthCheck(IUnitOfWork dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            _dbContext.Database.SqlQuery<int>($"SELECT 1");
            return Task.FromResult(HealthCheckResult.Healthy());
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy(ex.Message));
        }
    }
}