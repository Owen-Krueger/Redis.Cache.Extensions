using StackExchange.Redis;

namespace Redis.Sidecar.Cache;

public interface IRedisCache
{
    IDatabase Database { get; }

    Task<T?> GetAsync<T>(string key, Func<Task<T>>? function = null, TimeSpan? expiration = null);

    T? Get<T>(string key, Func<T>? function = null, TimeSpan? expiration = null);

    bool Set<T>(string key, T value, TimeSpan? expiration = null);

    Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiration = null);

    bool Delete(string key);

    Task<bool> DeleteAsync(string key);

    /// <inheritdoc />
    Task<T?> GetAsync<T>(string key, TimeSpan? expiration = null);
}