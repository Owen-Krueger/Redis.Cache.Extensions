using Moq;
using Moq.AutoMock;
using Redis.Cache.Testing;

namespace Redis.Cache.Tests;

public class RedisCacheTestExtensionsTests
{
    [Test]
    public void SetupGet_FromCache_ExpectedValue()
    {
        var mock = new AutoMocker();
        var redisMock = mock.GetMock<IRedisCache>();
        redisMock.SetupRedisGet<string?>(CacheKey)
            .Returns(ValueFromCache);
        var result = CacheFunction(redisMock.Object, CacheKey);
        
        Assert.That(result, Is.EqualTo(ValueFromCache));
    }
    
    [Test]
    public void SetupGet_WithoutKeyFromCache_ExpectedValue()
    {
        var mock = new AutoMocker();
        var redisMock = mock.GetMock<IRedisCache>();
        redisMock.SetupRedisGet<string?>()
            .Returns(ValueFromCache);
        var result = CacheFunction(redisMock.Object, CacheKey);
        
        Assert.That(result, Is.EqualTo(ValueFromCache));
    }
    
    [Test]
    public void SetupGet_FromInnerFunction_ExpectedValue()
    {
        var mock = new AutoMocker();
        var redisMock = mock.GetMock<IRedisCache>();
        redisMock.SetupRedisGet<string?>(CacheKey)
            .ReturnFromInnerFunction();
        var result = CacheFunction(redisMock.Object, CacheKey);
        
        Assert.That(result, Is.EqualTo(ValueFromInnerFunction));
    }
    
    [Test]
    public void SetupGet_WithoutKeyFromInnerFunction_ExpectedValue()
    {
        var mock = new AutoMocker();
        var redisMock = mock.GetMock<IRedisCache>();
        redisMock.SetupRedisGet<string?>()
            .ReturnFromInnerFunction();
        var result = CacheFunction(redisMock.Object, CacheKey);
        
        Assert.That(result, Is.EqualTo(ValueFromInnerFunction));
    }
    
    [Test]
    public async Task SetupGetAsync_FromCache_ExpectedValue()
    {
        var mock = new AutoMocker();
        var redisMock = mock.GetMock<IRedisCache>();
        redisMock.SetupRedisGetAsync<string?>(CacheKey)
            .ReturnsAsync(ValueFromCache);
        var result = await CacheFunctionAsync(redisMock.Object, CacheKey);
        
        Assert.That(result, Is.EqualTo(ValueFromCache));
    }
    
    [Test]
    public async Task SetupGetAsync_WithoutKeyFromCache_ExpectedValue()
    {
        var mock = new AutoMocker();
        var redisMock = mock.GetMock<IRedisCache>();
        redisMock.SetupRedisGetAsync<string?>()
            .ReturnsAsync(ValueFromCache);
        var result = await CacheFunctionAsync(redisMock.Object, CacheKey);
        
        Assert.That(result, Is.EqualTo(ValueFromCache));
    }
    
    [Test]
    public async Task SetupGetAsync_FromInnerFunction_ExpectedValue()
    {
        var mock = new AutoMocker();
        var redisMock = mock.GetMock<IRedisCache>();
        redisMock.SetupRedisGetAsync<string?>(CacheKey)
            .ReturnFromInnerFunction();
        var result = await CacheFunctionAsync(redisMock.Object, CacheKey);
        
        Assert.That(result, Is.EqualTo(ValueFromInnerFunction));
    }
    
    [Test]
    public async Task SetupGetAsync_WithoutKeyFromInnerFunction_ExpectedValue()
    {
        var mock = new AutoMocker();
        var redisMock = mock.GetMock<IRedisCache>();
        redisMock.SetupRedisGetAsync<string?>()
            .ReturnFromInnerFunction();
        var result = await CacheFunctionAsync(redisMock.Object, CacheKey);
        
        Assert.That(result, Is.EqualTo(ValueFromInnerFunction));
    }
    
    private static string? CacheFunction(IRedisCache redisCache, string key)
        => redisCache.Get(key, () => ValueFromInnerFunction, x => x != string.Empty);
    
    private static async Task<string?> CacheFunctionAsync(IRedisCache redisCache, string key)
        => await redisCache.GetAsync(key, () => Task.FromResult(ValueFromInnerFunction), x => x != string.Empty);

    private const string CacheKey = "Key";
    private const string ValueFromCache = "Cache Function Value";
    private const string ValueFromInnerFunction = "Inner Function Value";
}