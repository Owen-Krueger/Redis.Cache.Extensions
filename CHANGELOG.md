# 2.1.0 (2024-11-03)
## Features
- Added health check and health check extensions.

# 2.0.0 (2024-10-03)
## Features
- Added .NET 8.0 support.
- Added `Redis.Cache.Extensions.Testing` package for testing extensions.

## Breaking Changes
- Renamed package to `Redis.Cache.Extensions` from `Redis.Sidecar.Cache`.
- Dropped support for .NET 6.0.
- Swapped extensions for `WebApplicationBuilder` for extensions for `ServiceCollection`.

# 1.3.0 (2024-01-12)
## Features
- Added ability to specify configuration settings during instantiation of `RedisCache`

# 1.2.0 (2024-01-12)
## Features
- Added ability to specify host instead of setting up an `IDatabase` manually.

## Misc
- Clarify documentation that this package can be used with non-Sidecar caches.

# 1.1.1 (2024-01-11)
## Misc
- Clean up existing comments and add missing tags.

# 1.1.0 (2024-01-11)
## Features
- Added `condition` parameter to optionally save to the cache when a particular condition is met.
- Added additional overrides to allow for more flexible implementations.

## Misc
- Updated README to describe the package and its functionality.

# 1.0.1 (2024-01-10)
## Misc
- Added additional documentation on exceptions that can be raised during Get process.
- Removed temporary value used for testing.

# 1.0.0 (2024-01-09)
## Features
- Added extensions to use and manage a Redis Sidecar cache.
- Added WebAssemblyBuilder extensions for adding Redis cache to application services