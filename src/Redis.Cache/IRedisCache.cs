using Newtonsoft.Json;
using StackExchange.Redis;

namespace Redis.Cache;

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
    /// Gets the object at the key from the cache.
    /// Executes the <see cref="function"/> to get the object and add it in the cache.
    /// </summary>
    /// <param name="key">The identifier for the value to get from the cache.</param>
    /// <param name="function">Function to invoke to get the value to set in the cache, if the value isn't found in the cache.</param>
    /// <param name="condition">The condition that must be true to save the value to the cache.</param>
    /// <exception cref="JsonReaderException">Thrown if the string value in the cache cannot be converted to type <see cref="T"/>.</exception>
    /// <typeparam name="T">The type of object stored in the cache.</typeparam>
    /// <returns>The value of the object in the cache. Null if not found.</returns>
    T? Get<T>(string key, Func<T> function, Func<T, bool> condition);
    
    /// <summary>
    /// Gets the object at the key from the cache.
    /// If provided, executes the <see cref="function"/> to get the object and add it in the cache.
    /// If provided, only saves to the cache if the <see cref="condition"/> is met.
    /// </summary>
    /// <param name="key">The identifier for the value to get from the cache.</param>
    /// <param name="function">Optional. Function to invoke to get the value to set in the cache, if the value isn't found in the cache.</param>
    /// <param name="expiration">Optional. Time for the object to live in the cache. Defaults to 1 hour if not provided.</param>
    /// <param name="condition">Optional. The condition that must be true to save the value to the cache.</param>
    /// <exception cref="JsonReaderException">Thrown if the string value in the cache cannot be converted to type <see cref="T"/>.</exception>
    /// <typeparam name="T">The type of object stored in the cache.</typeparam>
    /// <returns>The value of the object in the cache. Null if not found.</returns>
    T? Get<T>(string key, Func<T>? function = null, TimeSpan? expiration = null, Func<T, bool>? condition = null);
    
    /// <summary>
    /// Gets the object at the key from the cache.
    /// Executes the <see cref="function"/> to get the object and add it in the cache.
    /// Only saves to the cache if the <see cref="condition"/> is met.
    /// </summary>
    /// <param name="key">The identifier for the value to get from the cache.</param>
    /// <param name="function">Function to invoke to get the value to set in the cache, if the value isn't found in the cache.</param>
    /// <param name="condition">The condition that must be true to save the value to the cache.</param>
    /// <exception cref="JsonReaderException">Thrown if the string value in the cache cannot be converted to type <see cref="T"/>.</exception>
    /// <typeparam name="T">The type of object stored in the cache.</typeparam>
    /// <returns>The value of the object in the cache. Null if not found.</returns>
    Task<T?> GetAsync<T>(string key, Func<Task<T>> function, Func<T, bool> condition);
    
    /// <summary>
    /// Gets the object at the key from the cache asynchronously.
    /// If <see cref="function"/> is provided, execute the function to get the object and add it in the cache.
    /// If provided, executes the <see cref="function"/> to get the object and add it in the cache.
    /// If provided, only saves to the cache if the <see cref="condition"/> is met.
    /// </summary>
    /// <param name="key">The identifier for the value to get from the cache.</param>
    /// <param name="function">Optional. Function to invoke to get the value to set in the cache, if the value isn't found in the cache.</param>
    /// <param name="expiration">Optional. Time for the object to live in the cache. Defaults to 1 hour if not provided.</param>
    /// <param name="condition">Optional. The condition that must be true to save the value to the cache.</param>
    /// <exception cref="JsonReaderException">Thrown if the string value in the cache cannot be converted to type <see cref="T"/>.</exception>
    /// <typeparam name="T">The type of object stored in the cache.</typeparam>
    /// <returns>The value of the object in the cache. Null if not found.</returns>
    Task<T?> GetAsync<T>(string key, Func<Task<T>>? function = null, TimeSpan? expiration = null, Func<T, bool>? condition = null);

    /// <summary>
    /// Sets the object at the key in the cache.
    /// </summary>
    /// <param name="key">The identifier for the value to set in the cache.</param>
    /// <param name="value">The value to set in the cache for the key.</param>
    /// <param name="expiration">Time for the object to live in the cache. Defaults to 1 hour if not provided.</param>
    /// <typeparam name="T">The type of object stored in the cache.</typeparam>
    /// <returns>True/False if the object was set in the cache successfully.</returns>
    bool Set<T>(string key, T value, TimeSpan expiration);
    
    /// <summary>
    /// Sets the object at the key in the cache.
    /// Only saves to the cache if the <see cref="condition"/> is met.
    /// </summary>
    /// <param name="key">The identifier for the value to set in the cache.</param>
    /// <param name="value">The value to set in the cache for the key.</param>
    /// <param name="condition">The condition that must be true to save the value to the cache.</param>
    /// <typeparam name="T">The type of object stored in the cache.</typeparam>
    /// <returns>True/False if the object was set in the cache successfully.</returns>
    bool Set<T>(string key, T value, Func<T, bool> condition);
    
    /// <summary>
    /// Sets the object at the key in the cache.
    /// If provided, only saves to the cache if the <see cref="condition"/> is met.
    /// </summary>
    /// <param name="key">The identifier for the value to set in the cache.</param>
    /// <param name="value">The value to set in the cache for the key.</param>
    /// <param name="expiration">Optional. Time for the object to live in the cache. Defaults to 1 hour if not provided.</param>
    /// <param name="condition">Optional. If provided, the condition that must be true to save the value to the cache.</param>
    /// <typeparam name="T">The type of object stored in the cache.</typeparam>
    /// <returns>True/False if the object was set in the cache successfully.</returns>
    bool Set<T>(string key, T value, TimeSpan? expiration = null, Func<T, bool>? condition = null);

    /// <summary>
    /// Sets the object at the key in the cache asynchronously.
    /// </summary>
    /// <param name="key">The identifier for the value to set in the cache.</param>
    /// <param name="value">The value to set in the cache for the key.</param>
    /// <param name="expiration">Optional. Time for the object to live in the cache. Defaults to 1 hour if not provided.</param>
    /// <typeparam name="T">The type of object stored in the cache.</typeparam>
    /// <returns>True/False if the object was set in the cache successfully.</returns>
    Task<bool> SetAsync<T>(string key, T value, TimeSpan expiration);
    
    /// <summary>
    /// Sets the object at the key in the cache asynchronously.
    /// Only saves to the cache if the <see cref="condition"/> is met.
    /// </summary>
    /// <param name="key">The identifier for the value to set in the cache.</param>
    /// <param name="value">The value to set in the cache for the key.</param>
    /// <param name="condition">The condition that must be true to save the value to the cache.</param>
    /// <typeparam name="T">The type of object stored in the cache.</typeparam>
    /// <returns>True/False if the object was set in the cache successfully.</returns>
    Task<bool> SetAsync<T>(string key, T value, Func<T, bool> condition);
    
    /// <summary>
    /// Sets the object at the key in the cache asynchronously.
    /// If provided, only saves to the cache if the <see cref="condition"/> is met.
    /// </summary>
    /// <param name="key">The identifier for the value to set in the cache.</param>
    /// <param name="value">The value to set in the cache for the key.</param>
    /// <param name="expiration">Optional. Time for the object to live in the cache. Defaults to 1 hour if not provided.</param>
    /// <param name="condition">Optional. The condition that must be true to save the value to the cache.</param>
    /// <typeparam name="T">The type of object stored in the cache.</typeparam>
    /// <returns>True/False if the object was set in the cache successfully.</returns>
    Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiration = null, Func<T, bool>? condition = null);

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