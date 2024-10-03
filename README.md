# Redis.Cache
[![.NET](https://github.com/Owen-Krueger/Redis.Sidecar.Cache/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Owen-Krueger/Redis.Sidecar.Cache/actions/workflows/dotnet.yml)

Redis.Cache is a package used to easily set up and use Redis caches within your application using the `StackExchange.Redis` package. You can use any Redis cache, including Sidecar caches, which is what will be used by default.

There are two packages available that contain a variety of methods are available to easily set up and utilize a cache within your application.

## RedisCache

`RedisCache` is a class available that takes care of setting up a connection to the Redis Sidecar for you, as well as provide useful methods for managing items in the cache.

### Available Methods

The methods available to be used are `Get`/`GetAsync`, `Set`/`SetAsync`, `Delete`/`DeleteAsync`.

##### Get/GetAsync

Required Parameters:
- key (`string`)
  - The identifier for the value to get from the cache.

Optional Parameters:
- function (`Func<T>`)
  - Function to invoke to get the value to set in the cache, if the value isn't found in the cache.
- expiration (`TimeSpan`)
  - Time for the object to live in the cache. Defaults to 1 hour if not provided.
- condition (`Func<T, bool>`)
  - The condition that must be true to save the value to the cache.

Gets the object at the key from the cache. `Get` does this synchronously, whereas `GetAsync` does this asynchronously.

`function`: If provided, executes the input function to get the object and add it in the cache.

`condition`: If provided, only saves to the cache if the input condition is met.

Returns the value from the cache, if found. Returns null if the value could not be found.

Examples:
``` C#
// Get
var value1 = redisCache.Get<int>("KeyName");
var value2 = redisCache.Get<int>("KeyName",() => 42);
var value3 = redisCache.Get<int>("KeyName",() => 42, TimeSpan.FromMinutes(20), x => x > 30);

// GetAsync
var value1 = await redisCache.GetAsync<int>("KeyName");
var value2 = await redisCache.GetAsync<int>("KeyName",() => 42);
var value3 = await redisCache.GetAsync<int>("KeyName",() => 42, TimeSpan.FromMinutes(20), x => x > 30); 
```

##### Set/SetAsync

Required Parameters:
- key (`string`)
  - The identifier for the value to get from the cache.
- value (`T`)
  - The value to set in the cache for the key.

Optional Parameters:
- expiration (`TimeSpan`)
  - Time for the object to live in the cache. Defaults to 1 hour if not provided.
- condition (`Func<T, bool>`)
  - The condition that must be true to save the value to the cache.

Sets the object at the key from the cache. `Set` does this synchronously, whereas `SetAsync` does this asynchronously.

`condition`: If provided, only saves to the cache if the input condition is met.

Sets the value for the key in the cache. Returns true if successful and false if not successful.

Examples:
``` C#
// Set
var value1 = redisCache.Set<int>("KeyName", 42);
var value2 = redisCache.Set<int>("KeyName", 42, Timespan.FromMinutes(20));
var value3 = redisCache.Set<int>("KeyName", 42, TimeSpan.FromMinutes(20), x => x > 30);
var value4 = redisCache.Set<int>("KeyName", 42, x => x > 30);

// SetAsync
var value1 = await redisCache.SetAsync<int>("KeyName", 42);
var value2 = await redisCache.SetAsync<int>("KeyName", 42, Timespan.FromMinutes(20));
var value3 = await redisCache.SetAsync<int>("KeyName", 42, TimeSpan.FromMinutes(20), x => x > 30);
var value4 = await redisCache.SetAsync<int>("KeyName", 42, x => x > 30);
```

##### Delete/DeleteAsync

Required Parameters:
- key (`string`)
  - The identifier for the value to get from the cache.

Removes the object at the key from the cache. `Delete` does this synchronously, whereas `DeleteAsync` does this asynchronously.

Returns true if successful and false if not successful.

Examples:
``` C#
// Delete
var deleted = redisCache.Delete("KeyName");

// DeleteAsync
var deleted = await redisCache.DeleteAsync("KeyName");
```

## ServiceCollection Extensions

This class provides extensions for `ServiceCollection` to automatically set up and register a `RedisCache` instance for your application to use.

By default, the `RedisCache` will attempt to find the Redis Sidecar container running on localhost.

Overloads are provided to provide an `IDatabase` or a specific host to set up the redis connection yourself.

Examples:
``` C#
services.AddRedisCache(); // Default

services.AddRedisCache(myDatabase); // `IDatabase` instance provided.

services.AddRedisCache("myhost"); // Host provided.

services.AddRedisCache(new ConfigurationOptions()); // Options provided.
```

## Redis.Cache.Testing

A `Redis.Cache.Testing` package is available to assist with unit testing Redis caches.

### Available Methods

The methods available to be used are `SetupRedisGet`/`SetupRedisGetAsync` and `ReturnFromInnerFunction`

### SetupRedisGet/SetupRedisGetAsync

This method specifies a setup on an `IRedisCache` mock for a call to the `Get` method.

A key string can optionally be provided. If provided, the setup will be for calls to `Get`/`GetAsync` for the provided key. If not provided, the setup will be for calls to `Get`/`GetAsync` for any provided key.

Examples:
``` C#
var redisMock = new Mock<IRedisCache>();

redisMock.SetupRedisGet<bool>("Key").Returns(true); // For setting up against `Get` method.
redisMock.SetupRedisGet<bool>().Returns(true); // Omitting a key sets up the mock against any key provided.
redisMock.SetupRedisGetAsync<bool>("Key").ReturnsAsync(true); // For setting up against `GetAsync` method.
redisMock.SetupRedisGetAsync<bool>().ReturnsAsync(true); // Omitting a key sets up the mock against any key provided.
```

### ReturnFromInnerFunction

This method specifies that the inner function in the `IRedisCache` mock's `Get`/`GetAsync` method will be invoked to return the value.

Examples:
``` C#
var redisMock = new Mock<IRedisCache>();

redisMock.SetupRedisGet<bool>("Key").ReturnsFromInnerFunction(); // Can be used on both `Get` and `GetAsync` calls.
```