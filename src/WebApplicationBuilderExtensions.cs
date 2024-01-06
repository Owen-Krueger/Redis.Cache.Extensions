using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Redis.Sidecar.Cache;

public static class WebApplicationBuilderExtensions
{
    public static void AddRedisCache(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<IRedisCache, RedisCache>();
    }
}