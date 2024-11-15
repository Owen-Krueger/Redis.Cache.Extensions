using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Redis.Cache.Extensions;

/// <summary>
/// Used for building health checks around redis caches.
/// </summary>
public static class RedisHealthCheckBuilder
{
    /// <summary>
    /// The default name used for a health check around enforced variables.
    /// </summary>
    public const string DefaultName = "redis_check";

    /// <summary>
    /// Adds a health check around redis caches to the provided <see cref="IHealthChecksBuilder"/>.
    /// </summary>
    /// <param name="builder">A builder used to register health checks.</param>
    /// <param name="name">The health check name.</param>
    /// <param name="failureStatus">
    /// The <see cref="HealthStatus"/> that should be reported when the health check reports a failure. If the provided value
    /// is <c>null</c>, then <see cref="HealthStatus.Unhealthy"/> will be reported.
    /// </param>
    /// <param name="tags">A list of tags that can be used for filtering health checks.</param>
    public static IHealthChecksBuilder AddRedisHealthCheck(
        this IHealthChecksBuilder builder,
        string? name = null,
        HealthStatus failureStatus = default,
        IEnumerable<string>? tags = default)
    {
        return builder.Add(new HealthCheckRegistration(
            name ?? DefaultName,
            sp => new RedisHealthCheck(sp),
            failureStatus,
            tags));
    }
}

/// <summary>
/// For performing health checks to verify a redis cache is connected.
/// </summary>
public class RedisHealthCheck(IServiceProvider serviceProvider) : IHealthCheck
{
    /// <summary>
    /// Runs the health check, returning the status of if the redis cache is connected.
    /// </summary>
    /// <param name="context">A context object associated with the current execution.</param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> that can be used to cancel the health check.</param>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that completes when the health check has finished, yielding the status of the component being checked.</returns>
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
    {
        try
        {
            var redisCache = serviceProvider.GetService<IRedisCache>();
            if (redisCache is null)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("`IRedisCache` not registered with ServiceProvider."));
            }

            return Task.FromResult(redisCache.ConnectionMultiplexer.IsConnected ?
                HealthCheckResult.Healthy("Redis cache connected.") :
                HealthCheckResult.Unhealthy("Redis cache not connected."));
        }
        catch (Exception e)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy($"Exception of type {e.GetType()} thrown."));
        }
    }
}