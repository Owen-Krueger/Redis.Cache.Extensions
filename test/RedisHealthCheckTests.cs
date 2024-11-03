using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq.AutoMock;
using StackExchange.Redis;

namespace Redis.Cache.Extensions.Tests;

public class RedisHealthCheckTests
{
    [Test]
    public async Task RedisHealthCheck_RedisConnected_Healthy()
    {
        var mock = new AutoMocker();
        var multiplexerMock = mock.GetMock<IConnectionMultiplexer>();
        multiplexerMock
            .SetupGet(x => x.IsConnected)
            .Returns(true);
        var databaseMock = mock.GetMock<IDatabase>();
        databaseMock
            .SetupGet(x => x.Multiplexer)
            .Returns(multiplexerMock.Object);
        var redisMock = mock.GetMock<IRedisCache>();
        redisMock
            .SetupGet(x => x.Database)
            .Returns(databaseMock.Object);
        mock.GetMock<IServiceProvider>()
            .Setup(x => x.GetService(typeof(IRedisCache)))
            .Returns(redisMock.Object);
        var healthCheck = mock.CreateInstance<RedisHealthCheck>();

        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());
        Assert.That(result.Status, Is.EqualTo(HealthStatus.Healthy));
    }

    [Test]
    public async Task RedisHealthCheck_RedisNotConnected_Unhealthy()
    {
        var mock = new AutoMocker();
        var multiplexerMock = mock.GetMock<IConnectionMultiplexer>();
        multiplexerMock
            .SetupGet(x => x.IsConnected)
            .Returns(false);
        var databaseMock = mock.GetMock<IDatabase>();
        databaseMock
            .SetupGet(x => x.Multiplexer)
            .Returns(multiplexerMock.Object);
        var redisMock = mock.GetMock<IRedisCache>();
        redisMock
            .SetupGet(x => x.Database)
            .Returns(databaseMock.Object);
        mock.GetMock<IServiceProvider>()
            .Setup(x => x.GetService(typeof(IRedisCache)))
            .Returns(redisMock.Object);
        var healthCheck = mock.CreateInstance<RedisHealthCheck>();

        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());
        Assert.That(result.Status, Is.EqualTo(HealthStatus.Unhealthy));
    }

    [Test]
    public async Task RedisHealthCheck_CacheNotInServices_Unhealthy()
    {
        var mock = new AutoMocker();
        mock.GetMock<IServiceProvider>()
            .Setup(x => x.GetService(typeof(IRedisCache)))
            .Returns((object?)null);
        var healthCheck = mock.CreateInstance<RedisHealthCheck>();

        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());
        Assert.That(result.Status, Is.EqualTo(HealthStatus.Unhealthy));
    }
}