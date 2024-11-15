using AutoFixture;
using Moq;
using Moq.AutoMock;
using Redis.Cache.Extensions.Testing;

namespace Redis.Cache.Extensions.Tests;

public class RedisCacheTestExtensionsTests
{
    private IFixture fixture;

    [SetUp]
    public void Initialize()
    {
        fixture = new Fixture();
    }
    
    [Test]
    public void SetupGet_WithKeyFromCache_ExpectedValue()
    {
        var mock = new AutoMocker();
        var key = fixture.Create<string>();
        var value = fixture.Create<string>();
        var redisMock = mock.GetMock<IRedisCache>();
        redisMock.SetupRedisGet<string>(key)
            .Returns(value);
        var result = redisMock.Object
            .Get<string>(key);
        
        Assert.That(result, Is.EqualTo(value));
    }
    
    [Test]
    public void SetupGet_WithoutKeyFromCache_ExpectedValue()
    {
        var mock = new AutoMocker();
        var key = fixture.Create<string>();
        var value = fixture.Create<string>();
        var redisMock = mock.GetMock<IRedisCache>();
        redisMock.SetupRedisGet<string>()
            .Returns(value);
        var result = redisMock.Object
            .Get<string>(key);
        
        Assert.That(result, Is.EqualTo(value));
    }
    
    [Test]
    public void SetupGet_WithKeyFromInnerFunction_ExpectedValue()
    {
        var mock = new AutoMocker();
        var key = fixture.Create<string>();
        var value = fixture.Create<string>();
        var redisMock = mock.GetMock<IRedisCache>();
        redisMock.SetupRedisGet<string?>(key)
            .ReturnFromInnerFunction();
        var result = redisMock.Object
            .Get(key, () => value);
        
        Assert.That(result, Is.EqualTo(value));
    }
    
    [Test]
    public void SetupGet_WithoutKeyFromInnerFunction_ExpectedValue()
    {
        var mock = new AutoMocker();
        var key = fixture.Create<string>();
        var value = fixture.Create<string>();
        var redisMock = mock.GetMock<IRedisCache>();
        redisMock.SetupRedisGet<string?>()
            .ReturnFromInnerFunction();
        var result = redisMock.Object
            .Get(key, () => value);
        
        Assert.That(result, Is.EqualTo(value));
    }
    
    [Test]
    public async Task SetupGetAsync_WithKeyFromCache_ExpectedValue()
    {
        var mock = new AutoMocker();
        var key = fixture.Create<string>();
        var value = fixture.Create<string>();
        var redisMock = mock.GetMock<IRedisCache>();
        redisMock.SetupRedisGetAsync<string>(key)
            .ReturnsAsync(value);
        var result = await redisMock.Object
            .GetAsync<string>(key);
        
        Assert.That(result, Is.EqualTo(value));
    }
    
    [Test]
    public async Task SetupGetAsync_WithoutKeyFromCache_ExpectedValue()
    {
        var mock = new AutoMocker();
        var key = fixture.Create<string>();
        var value = fixture.Create<string>();
        var redisMock = mock.GetMock<IRedisCache>();
        redisMock.SetupRedisGetAsync<string>()
            .ReturnsAsync(value);
        var result = await redisMock.Object
            .GetAsync<string>(key);
        
        Assert.That(result, Is.EqualTo(value));
    }
    
    [Test]
    public async Task SetupGetAsync_WithKeyFromInnerFunction_ExpectedValue()
    {
        var mock = new AutoMocker();
        var key = fixture.Create<string>();
        var value = fixture.Create<string>();
        var redisMock = mock.GetMock<IRedisCache>();
        redisMock.SetupRedisGetAsync<string?>(key)
            .ReturnFromInnerFunction();
        var result = await redisMock.Object
            .GetAsync(key, () => Task.FromResult(value));
        
        Assert.That(result, Is.EqualTo(value));
    }
    
    [Test]
    public async Task SetupGetAsync_WithoutKeyFromInnerFunction_ExpectedValue()
    {
        var mock = new AutoMocker();
        var key = fixture.Create<string>();
        var value = fixture.Create<string>();
        var redisMock = mock.GetMock<IRedisCache>();
        redisMock.SetupRedisGetAsync<string?>()
            .ReturnFromInnerFunction();
        var result = await redisMock.Object
            .GetAsync(key, () => Task.FromResult(value));
        
        Assert.That(result, Is.EqualTo(value));
    }
}