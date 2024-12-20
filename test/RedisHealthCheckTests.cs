﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using Moq.AutoMock;
using StackExchange.Redis;

namespace Redis.Cache.Extensions.Tests;

public class RedisHealthCheckTests
{
    [Test]
    public void AddRedisHealthCheck_HealthCheckRegistered_Success()
    {
        var mock = new AutoMocker();
        var builderMock = mock.GetMock<IHealthChecksBuilder>();
        List<HealthCheckRegistration> registrations = [];
        builderMock
            .Setup(x => x.Add(It.IsAny<HealthCheckRegistration>()))
            .Callback<HealthCheckRegistration>(x => registrations.Add(x))
            .Returns(builderMock.Object);
        builderMock.Object.AddRedisHealthCheck();

        Assert.That(registrations, Has.One.Items);
        var registration = registrations[0];
        Assert.Multiple(() =>
        {
            Assert.That(registration.Name, Is.EqualTo(RedisHealthCheckBuilder.DefaultName));
            Assert.That(registration.FailureStatus, Is.EqualTo(HealthStatus.Unhealthy));
            Assert.That(registration.Tags, Is.Empty);
        });
    }

    [Test]
    public async Task RedisHealthCheck_RedisConnected_Healthy()
    {
        var mock = new AutoMocker();
        var connectionMultiplexerMock = mock.GetMock<IConnectionMultiplexer>();
        connectionMultiplexerMock
            .SetupGet(x => x.IsConnected)
            .Returns(true);
        var redisMock = mock.GetMock<IRedisCache>();
        redisMock
            .SetupGet(x => x.ConnectionMultiplexer)
            .Returns(connectionMultiplexerMock.Object);
        mock.GetMock<IServiceProvider>()
            .Setup(x => x.GetService(typeof(IRedisCache)))
            .Returns(redisMock.Object);
        var healthCheck = mock.CreateInstance<RedisHealthCheck>();
        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());
        Assert.That(result.Status, Is.EqualTo(HealthStatus.Healthy));
    }

    [Test]
    public async Task RedisHealthCheck_RedisCacheNotRegistered_Unhealthy()
    {
        var mock = new AutoMocker();
        mock.GetMock<IServiceProvider>()
            .Setup(x => x.GetService(typeof(IRedisCache)))
            .Returns((IRedisCache)null!);
        var healthCheck = mock.CreateInstance<RedisHealthCheck>();
        var result = await healthCheck.CheckHealthAsync(new HealthCheckContext());
        Assert.That(result.Status, Is.EqualTo(HealthStatus.Unhealthy));
    }

    [Test]
    public async Task RedisHealthCheck_RedisNotConnected_Unhealthy()
    {
        var mock = new AutoMocker();
        var connectionMultiplexerMock = mock.GetMock<IConnectionMultiplexer>();
        connectionMultiplexerMock
            .SetupGet(x => x.IsConnected)
            .Returns(false);
        var redisMock = mock.GetMock<IRedisCache>();
        redisMock
            .SetupGet(x => x.ConnectionMultiplexer)
            .Returns(connectionMultiplexerMock.Object);
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