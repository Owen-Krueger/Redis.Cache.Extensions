using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Redis.Cache;

/// <summary>
/// Extension methods for <see cref="ServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a <see cref="RedisCache"/> to the service collection.
    /// Inner <see cref="IDatabase"/> created automatically pointed at a localhost instance.
    /// </summary>
    public static void AddRedisCache(this ServiceCollection services) => 
        services.AddTransient<IRedisCache, RedisCache>();
    
    /// <summary>
    /// Adds a <see cref="RedisCache"/> to the service collection.
    /// Inner <see cref="IDatabase"/> created automatically pointed at a specified host instance.
    /// </summary>
    public static void AddRedisCache(this ServiceCollection services, string host) => 
        services.AddTransient<IRedisCache>(_ => new RedisCache(host));

    /// <summary>
    /// Adds a <see cref="RedisCache"/> to the service collection.
    /// Inner <see cref="IDatabase"/> created with provided options.
    /// </summary>
    public static void AddRedisCache(this ServiceCollection services, ConfigurationOptions options) =>
        services.AddTransient<IRedisCache>(_ => new RedisCache(options));
    
    /// <summary>
    /// Adds a <see cref="RedisCache"/> to the service collection.
    /// Inner <see cref="IDatabase"/> provided as a parameter.
    /// </summary>
    public static void AddRedisCache(this ServiceCollection services, IDatabase database) =>
        services.AddTransient<IRedisCache>(_ => new RedisCache(database));
}