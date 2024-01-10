using Newtonsoft.Json;
using StackExchange.Redis;

namespace Redis.Sidecar.Cache;

/// <inheritdoc />
public class RedisCache(IDatabase database) : IRedisCache
{
    private const int DefaultRedisExpirationMinutes = 60;

    /// <summary>
    /// Instantiates a new instance of <see cref="RedisCache"/> with a localhost Redis database.
    /// </summary>
    public RedisCache() : this(ConfigureRedis()) { }

    /// <inheritdoc />
    public IDatabase Database => database;

    /// <summary>
    /// Method handling configuration of redis.
    /// </summary>
    private static IDatabase ConfigureRedis()
    {
        var redis = ConnectionMultiplexer.Connect("localhost");
        return redis.GetDatabase();
    }
    
    /// <inheritdoc />
    public T? Get<T>(string key, Func<T>? function = null, TimeSpan? expiration = null)
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
            Set(key, value, expiration);
        }

        return value;
    }

    /// <inheritdoc />
    public async Task<T?> GetAsync<T>(string key, Func<Task<T>>? function = null, TimeSpan? expiration = null)
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
            await SetAsync(key, value, expiration);
        }

        return value;
    }
    
    /// <inheritdoc />
    public bool Set<T>(string key, T value, TimeSpan? expiration = null)
    {
        var jsonValue = JsonConvert.SerializeObject(value);
        return database.StringSet(key, jsonValue, expiration ?? TimeSpan.FromMinutes(DefaultRedisExpirationMinutes));
    }

    /// <inheritdoc />
    public async Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var jsonValue = JsonConvert.SerializeObject(value);
        return await database.StringSetAsync(key, jsonValue, expiration ?? TimeSpan.FromMinutes(DefaultRedisExpirationMinutes));
    }

    /// <inheritdoc />
    public bool Delete(string key) => database.StringGetDelete(key).HasValue;

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(string key) => (await database.StringGetDeleteAsync(key)).HasValue;
}