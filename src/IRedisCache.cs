using StackExchange.Redis;

namespace Redis.Sidecar.Cache;

/// <summary>
/// For interacting with a Redis cache.
/// </summary>
public interface IRedisCache
{
    /// <summary>
    /// Gets the <see cref="IDatabase"/> instance to directly interact with the Redis cache.
    /// </summary>
    IDatabase Database { get; }
    
    /// <summary>
    /// Gets the object at the key from the cache. If <see cref="function"/> is provided, execute the function to get
    /// the object and add it in the cache.
    /// </summary>
    /// <param name="key">The identifier for the value to get from the cache.</param>
    /// <param name="function">Optional. Function to invoke to get the value to set in the cache, if the value isn't found in the cache.</param>
    /// <param name="expiration">Optional. Time for the object to live in the cache. Defaults to 1 hour if not provided.</param>
    /// <typeparam name="T">The type of object stored in the cache.</typeparam>
    /// <returns>The value of the object in the cache. Null if not found.</returns>
    T? Get<T>(string key, Func<T>? function = null, TimeSpan? expiration = null);
    
    /// <summary>
    /// Gets the object at the key from the cache asynchronously. If <see cref="function"/> is provided, execute the
    /// function to get the object and add it in the cache.
    /// </summary>
    /// <param name="key">The identifier for the value to get from the cache.</param>
    /// <param name="function">Optional. Function to invoke to get the value to set in the cache, if the value isn't found in the cache.</param>
    /// <param name="expiration">Optional. Time for the object to live in the cache. Defaults to 1 hour if not provided.</param>
    /// <typeparam name="T">The type of object stored in the cache.</typeparam>
    /// <returns>The value of the object in the cache. Null if not found.</returns>
    Task<T?> GetAsync<T>(string key, Func<Task<T>>? function = null, TimeSpan? expiration = null);

    /// <summary>
    /// Sets the object at the key in the cache.
    /// </summary>
    /// <param name="key">The identifier for the value to set in the cache.</param>
    /// <param name="value">The value to set for the key.</param>
    /// <param name="expiration">Optional. Time for the object to live in the cache. Defaults to 1 hour if not provided.</param>
    /// <typeparam name="T">The type of object stored in the cache.</typeparam>
    /// <returns>True/False if the object was set in the cache successfully.</returns>
    bool Set<T>(string key, T value, TimeSpan? expiration = null);

    /// <summary>
    /// Sets the object at the key in the cache asynchronously.
    /// </summary>
    /// <param name="key">The identifier for the value to set in the cache.</param>
    /// <param name="value">The value to set for the key.</param>
    /// <param name="expiration">Optional. Time for the object to live in the cache. Defaults to 1 hour if not provided.</param>
    /// <typeparam name="T">The type of object stored in the cache.</typeparam>
    /// <returns>True/False if the object was set in the cache successfully.</returns>
    Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiration = null);

    /// <summary>
    /// Deletes the value out of the cache at the specified key.
    /// </summary>
    /// <param name="key">The identifier for the value to remove out of the cache.</param>
    /// <returns>True/False if the object was deleted out of the cache.</returns>
    bool Delete(string key);

    /// <summary>
    /// Deletes the value out of the cache at the specified key asynchronously.
    /// </summary>
    /// <param name="key">The identifier for the value to remove out of the cache.</param>
    /// <returns>True/False if the object was deleted out of the cache.</returns>
    Task<bool> DeleteAsync(string key);

}