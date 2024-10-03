using Newtonsoft.Json;
using StackExchange.Redis;

namespace Redis.Cache.Extensions;

/// <inheritdoc />
public class RedisCache(IDatabase database) : IRedisCache
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
    public RedisCache(string host) : this(ConnectionMultiplexer.Connect(host).GetDatabase()) { }

    /// <summary>
    /// Instantiates a new instance of <see cref="RedisCache"/> using the specified options for the Redis database.
    /// </summary>
    /// <param name="options"></param>
    public RedisCache(ConfigurationOptions options) : this(ConnectionMultiplexer.Connect(options).GetDatabase()) { }
    
    /// <inheritdoc />
    public IDatabase Database => database;

    /// <inheritdoc />
    public T? Get<T>(string key, Func<T> function, Func<T, bool> condition) =>
        Get(key, function, null, condition);

    /// <inheritdoc />
    public T? Get<T>(string key, Func<T>? function = null, TimeSpan? expiration = null, Func<T, bool>? condition = null)
    {
        var redisValue = database.StringGet(key);
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
        var redisValue = await database.StringGetAsync(key);
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
        return database.StringSet(key, jsonValue, expiration ?? TimeSpan.FromMinutes(DefaultRedisExpirationMinutes));

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
        return await database.StringSetAsync(key, jsonValue, expiration ?? TimeSpan.FromMinutes(DefaultRedisExpirationMinutes));
    }

    /// <inheritdoc />
    public bool Delete(string key) => database.StringGetDelete(key).HasValue;

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(string key) => (await database.StringGetDeleteAsync(key)).HasValue;
}