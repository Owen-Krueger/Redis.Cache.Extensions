using Newtonsoft.Json;
using StackExchange.Redis;

namespace Redis.Cache.Extensions;

/// <inheritdoc />
public class RedisCache(IConnectionMultiplexer connectionMultiplexer) : IRedisCache
{
    private const int DefaultRedisExpirationMinutes = 60;

    /// <summary>
    /// Instantiates a new instance of <see cref="RedisCache"/> with a localhost Redis database.
    /// </summary>
    public RedisCache() : this("localhost") { }

    /// <summary>
    /// Instantiates a new instance of <see cref="RedisCache"/> with a specified host for the Redis database.
    /// </summary>
    /// <param name="host"></param>
    public RedisCache(string host) : this(StackExchange.Redis.ConnectionMultiplexer.Connect(host)) { }

    /// <summary>
    /// Instantiates a new instance of <see cref="RedisCache"/> using the specified options for the Redis database.
    /// </summary>
    /// <param name="options"></param>
    public RedisCache(ConfigurationOptions options) : this(StackExchange.Redis.ConnectionMultiplexer.Connect(options)) { }

    /// <inheritdoc />
    public IConnectionMultiplexer ConnectionMultiplexer => connectionMultiplexer;
    
    /// <inheritdoc />
    public IDatabase Database => connectionMultiplexer.GetDatabase();

    /// <inheritdoc />
    public T? Get<T>(string key, Func<T> function, Func<T, bool> condition) =>
        Get(key, function, null, condition);

    /// <inheritdoc />
    public T? Get<T>(string key, Func<T>? function = null, TimeSpan? expiration = null, Func<T, bool>? condition = null)
    {
        var redisValue = Database.StringGet(key);
        if (redisValue.HasValue)
        {
            return JsonConvert.DeserializeObject<T>(redisValue!);
        }

        if (function is null)
        {
            return default;
        }

        var value = function.Invoke();
        if (value is not null)
        {
            Set(key, value, expiration, condition);
        }

        return value;
    }
    
    /// <inheritdoc />
    public async Task<T?> GetAsync<T>(string key, Func<Task<T>> function, Func<T, bool> condition) =>
        await GetAsync(key, function, null, condition);

    /// <inheritdoc />
    public async Task<T?> GetAsync<T>(string key, Func<Task<T>>? function = null, TimeSpan? expiration = null, Func<T, bool>? condition = null)
    {
        var redisValue = await Database.StringGetAsync(key);
        if (redisValue.HasValue)
        {
            return JsonConvert.DeserializeObject<T>(redisValue!);
        }

        if (function is null)
        {
            return default;
        }

        var value = await function.Invoke();
        if (value is not null)
        {
            await SetAsync(key, value, expiration, condition);
        }

        return value;
    }

    /// <inheritdoc />
    public bool Set<T>(string key, T value, TimeSpan expiration) =>
        Set(key, value, expiration, null);

    /// <inheritdoc />
    public bool Set<T>(string key, T value, Func<T, bool> condition) =>
        Set(key, value, null, condition);

    /// <inheritdoc />
    public bool Set<T>(string key, T value, TimeSpan? expiration = null, Func<T, bool>? condition = null)
    {
        if (condition is not null && !condition.Invoke(value))
        {
            return false;
        }
        
        var jsonValue = JsonConvert.SerializeObject(value);
        return Database.StringSet(key, jsonValue, expiration ?? TimeSpan.FromMinutes(DefaultRedisExpirationMinutes));
    }

    /// <inheritdoc />
    public async Task<bool> SetAsync<T>(string key, T value, TimeSpan expiration) =>
        await SetAsync(key, value, expiration, null);

    /// <inheritdoc />
    public async Task<bool> SetAsync<T>(string key, T value, Func<T, bool> condition) =>
        await SetAsync(key, value, null, condition);

    /// <inheritdoc />
    public async Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiration = null, Func<T, bool>? condition = null)
    {
        if (condition is not null && !condition.Invoke(value))
        {
            return false;
        }
        
        var jsonValue = JsonConvert.SerializeObject(value);
        return await Database.StringSetAsync(key, jsonValue, expiration ?? TimeSpan.FromMinutes(DefaultRedisExpirationMinutes));
    }

    /// <inheritdoc />
    public bool Delete(string key) => Database.StringGetDelete(key).HasValue;

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(string key) => (await Database.StringGetDeleteAsync(key)).HasValue;
}