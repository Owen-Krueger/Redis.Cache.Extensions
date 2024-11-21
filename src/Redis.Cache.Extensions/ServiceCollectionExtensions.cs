using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Redis.Cache.Extensions;

/// <summary>
/// Extension methods for <see cref="ServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a <see cref="RedisCache"/> to the service collection.
    /// Inner <see cref="IConnectionMultiplexer"/> created automatically pointed at a localhost instance.
    /// </summary>
    public static void AddRedisCache(this ServiceCollection services) => 
        services.AddSingleton<IRedisCache, RedisCache>();
    
    /// <summary>
    /// Adds a <see cref="RedisCache"/> to the service collection.
    /// Inner <see cref="IConnectionMultiplexer"/> created automatically pointed at a specified host instance.
    /// </summary>
    public static void AddRedisCache(this ServiceCollection services, string host) => 
        services.AddSingleton<IRedisCache>(_ => new RedisCache(host));

    /// <summary>
    /// Adds a <see cref="RedisCache"/> to the service collection.
    /// Inner <see cref="IConnectionMultiplexer"/> created with provided options.
    /// </summary>
    public static void AddRedisCache(this ServiceCollection services, ConfigurationOptions options) =>
        services.AddSingleton<IRedisCache>(_ => new RedisCache(options));
    
    /// <summary>
    /// Adds a <see cref="RedisCache"/> to the service collection.
    /// Inner <see cref="IConnectionMultiplexer"/> provided as a parameter.
    /// </summary>
    public static void AddRedisCache(this ServiceCollection services, IConnectionMultiplexer multiplexer) =>
        services.AddSingleton<IRedisCache>(_ => new RedisCache(multiplexer));
}