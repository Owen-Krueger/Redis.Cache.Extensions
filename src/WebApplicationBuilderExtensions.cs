using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Redis.Sidecar.Cache;

/// <summary>
/// Extension methods for <see cref="WebApplicationBuilder"/>.
/// </summary>
public static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// Adds a <see cref="RedisCache"/> to the service collection.
    /// Inner <see cref="IDatabase"/> created automatically pointed at a localhost instance.
    /// </summary>
    public static void AddRedisCache(this WebApplicationBuilder builder) => 
        builder.Services.AddTransient<IRedisCache, RedisCache>();
    
    /// <summary>
    /// Adds a <see cref="RedisCache"/> to the service collection.
    /// Inner <see cref="IDatabase"/> created automatically pointed at a specified host instance.
    /// </summary>
    public static void AddRedisCache(this WebApplicationBuilder builder, string host) => 
        builder.Services.AddTransient<IRedisCache>(_ => new RedisCache(host));

    /// <summary>
    /// Adds a <see cref="RedisCache"/> to the service collection.
    /// Inner <see cref="IDatabase"/> provided as a parameter.
    /// </summary>
    public static void AddRedisCache(this WebApplicationBuilder builder, IDatabase database) =>
        builder.Services.AddTransient<IRedisCache>(_ => new RedisCache(database));
}