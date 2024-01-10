using Moq;
using Moq.AutoMock;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Redis.Sidecar.Cache.Tests;

public class RedisCacheTests
{
    [Test]
    public void Get_ValueFound_ValueReturned()
    {
        var mock = new AutoMocker();
        var databaseMock = mock.GetMock<IDatabase>();
        databaseMock.Setup(x => x.StringGet(Key, CommandFlags.None)).Returns(Value);
        var functionMock = new Mock<Func<int>>();
        var service = mock.CreateInstance<RedisCache>();

        var response = service.Get(Key, functionMock.Object);
        Assert.That(response, Is.EqualTo(Value));
        databaseMock.Verify(x => x.StringSet(Key, It.IsAny<RedisValue>(), redisCacheMinutes, false, When.Always, CommandFlags.None), Times.Never);
        functionMock.Verify(x => x.Invoke(), Times.Never);
    }

    [Test]
    public void Get_ValueNotFound_FunctionCalled()
    {
        var mock = new AutoMocker();
        var databaseMock = mock.GetMock<IDatabase>();
        databaseMock.Setup(x => x.StringGet(Key, CommandFlags.None)).Returns(new RedisValue());
        var functionMock = new Mock<Func<int>>();
        functionMock.Setup(x => x.Invoke()).Returns(Value);
        var service = mock.CreateInstance<RedisCache>();

        var response = service.Get(Key, functionMock.Object, redisCacheMinutes);
        Assert.That(response, Is.EqualTo(Value));
        databaseMock.Verify(x => x.StringSet(Key, Value.ToString(), redisCacheMinutes, false, When.Always, CommandFlags.None), Times.Once);
    }
    
    [Test]
    public void Get_ExpirationNotProvided_DefaultExpirationUsed()
    {
        var mock = new AutoMocker();
        var databaseMock = mock.GetMock<IDatabase>();
        databaseMock.Setup(x => x.StringGet(Key, CommandFlags.None)).Returns(new RedisValue());
        var functionMock = new Mock<Func<int>>();
        functionMock.Setup(x => x.Invoke()).Returns(Value);
        var service = mock.CreateInstance<RedisCache>();

        var response = service.Get(Key, functionMock.Object);
        Assert.That(response, Is.EqualTo(Value));
        databaseMock.Verify(x => x.StringSet(Key, Value.ToString(), TimeSpan.FromMinutes(60), false, When.Always, CommandFlags.None), Times.Once);
    }

    [Test]
    public void Get_ValueQueryFailed_ValueNotSaved()
    {
        var mock = new AutoMocker();
        var databaseMock = mock.GetMock<IDatabase>();
        databaseMock.Setup(x => x.StringGet(Key, CommandFlags.None)).Returns(new RedisValue());
        var functionMock = new Mock<Func<int?>>();
        functionMock.Setup(x => x.Invoke()).Returns((int?)null);
        var service = mock.CreateInstance<RedisCache>();

        var response = service.Get(Key, functionMock.Object);
        Assert.That(response, Is.Null);
        databaseMock.Verify(x => x.StringSet(Key, It.IsAny<RedisValue>(), redisCacheMinutes, false, When.Always, CommandFlags.None), Times.Never);
    }
    
    [Test]
    public void Get_NoFunctionProvided_DefaultReturned()
    {
        var mock = new AutoMocker();
        var databaseMock = mock.GetMock<IDatabase>();
        databaseMock
            .Setup(x => x.StringGet(Key, CommandFlags.None))
            .Returns(new RedisValue());
        var service = mock.CreateInstance<RedisCache>();

        var response = service.Get<object>(Key);
        Assert.That(response, Is.Null);
        databaseMock.Verify(x => x.StringSet(Key, It.IsAny<RedisValue>(), redisCacheMinutes, false, When.Always, CommandFlags.None), Times.Never);
    }

    [Test]
    public void Get_ValueNotSaved_ValueReturned()
    {
        var mock = new AutoMocker();
        var databaseMock = mock.GetMock<IDatabase>();
        databaseMock
            .Setup(x => x.StringGet(Key, CommandFlags.None))
            .Returns(new RedisValue());
        var functionMock = new Mock<Func<int>>();
        functionMock.Setup(x => x.Invoke()).Returns(Value);
        databaseMock
            .Setup(x => x.StringSet(Key, It.IsAny<RedisValue>(), redisCacheMinutes, false, When.Always, CommandFlags.None))
            .Returns(false);
        var service = mock.CreateInstance<RedisCache>();

        var response = service.Get(Key, functionMock.Object);
        Assert.That(response, Is.EqualTo(Value));
    }

    [Test]
    public void Get_BadJson_JsonReaderExceptionThrown()
    {
        var mock = new AutoMocker();
        var databaseMock = mock.GetMock<IDatabase>();
        databaseMock.Setup(x => x.StringGet(Key, CommandFlags.None)).Returns("BAD JSON PAYLOAD");
        var service = mock.CreateInstance<RedisCache>();

        Assert.Throws<JsonReaderException>(() => service.Get<int>(Key));
    }
    
    [Test]
    public async Task GetAsync_ValueFound_ValueReturned()
    {
        var mock = new AutoMocker();
        var databaseMock = mock.GetMock<IDatabase>();
        databaseMock
            .Setup(x => x.StringGetAsync(Key, CommandFlags.None))
            .ReturnsAsync(Value);
        var functionMock = new Mock<Func<Task<int>>>();
        var service = mock.CreateInstance<RedisCache>();

        var response = await service.GetAsync(Key, functionMock.Object);
        Assert.That(response, Is.EqualTo(Value));
        databaseMock.Verify(x => x.StringSet(Key, It.IsAny<RedisValue>(), redisCacheMinutes, false, When.Always, CommandFlags.None), Times.Never);
        functionMock.Verify(x => x.Invoke(), Times.Never);
    }

    [Test]
    public async Task GetAsync_ValueNotFound_FunctionCalled()
    {
        var mock = new AutoMocker();
        var databaseMock = mock.GetMock<IDatabase>();
        databaseMock
            .Setup(x => x.StringGetAsync(Key, CommandFlags.None))
            .ReturnsAsync(new RedisValue());
        var functionMock = new Mock<Func<Task<int>>>();
        functionMock.Setup(x => x.Invoke()).ReturnsAsync(Value);
        var service = mock.CreateInstance<RedisCache>();

        var response = await service.GetAsync(Key, functionMock.Object, redisCacheMinutes);
        Assert.That(response, Is.EqualTo(Value));
        databaseMock.Verify(x => x.StringSetAsync(Key, Value.ToString(), redisCacheMinutes, false, When.Always, CommandFlags.None), Times.Once);
    }
    
    [Test]
    public async Task GetAsync_ExpirationNotProvided_DefaultExpirationUsed()
    {
        var mock = new AutoMocker();
        var databaseMock = mock.GetMock<IDatabase>();
        databaseMock
            .Setup(x => x.StringGetAsync(Key, CommandFlags.None))
            .ReturnsAsync(new RedisValue());
        var functionMock = new Mock<Func<Task<int>>>();
        functionMock.Setup(x => x.Invoke()).ReturnsAsync(Value);
        var service = mock.CreateInstance<RedisCache>();

        var response = await service.GetAsync(Key, functionMock.Object);
        Assert.That(response, Is.EqualTo(Value));
        databaseMock.Verify(x => x.StringSetAsync(Key, Value.ToString(), TimeSpan.FromMinutes(60), false, When.Always, CommandFlags.None), Times.Once);
    }

    [Test]
    public async Task GetAsync_ValueQueryFailed_ValueNotSaved()
    {
        var mock = new AutoMocker();
        var databaseMock = mock.GetMock<IDatabase>();
        databaseMock
            .Setup(x => x.StringGetAsync(Key, CommandFlags.None))
            .ReturnsAsync(new RedisValue());
        var functionMock = new Mock<Func<Task<int?>>>();
        functionMock.Setup(x => x.Invoke()).ReturnsAsync((int?)null);
        var service = mock.CreateInstance<RedisCache>();

        var response = await service.GetAsync(Key, functionMock.Object);
        Assert.That(response, Is.Null);
        databaseMock.Verify(x => x.StringSet(Key, It.IsAny<RedisValue>(), redisCacheMinutes, false, When.Always, CommandFlags.None), Times.Never);
    }
    
    [Test]
    public async Task GetAsync_NoFunctionProvided_DefaultReturned()
    {
        var mock = new AutoMocker();
        var databaseMock = mock.GetMock<IDatabase>();
        databaseMock
            .Setup(x => x.StringGetAsync(Key, CommandFlags.None))
            .ReturnsAsync(new RedisValue());
        var service = mock.CreateInstance<RedisCache>();

        var response = await service.GetAsync<object>(Key);
        Assert.That(response, Is.Null);
        databaseMock.Verify(x => x.StringSet(Key, It.IsAny<RedisValue>(), redisCacheMinutes, false, When.Always, CommandFlags.None), Times.Never);
    }

    [Test]
    public async Task GetAsync_ValueNotSaved_ValueReturned()
    {
        var mock = new AutoMocker();
        var databaseMock = mock.GetMock<IDatabase>();
        databaseMock
            .Setup(x => x.StringGetAsync(Key, CommandFlags.None))
            .ReturnsAsync(new RedisValue());
        var functionMock = new Mock<Func<Task<int>>>();
        functionMock.Setup(x => x.Invoke()).ReturnsAsync(Value);
        databaseMock
            .Setup(x => x.StringSet(Key, It.IsAny<RedisValue>(), redisCacheMinutes, false, When.Always, CommandFlags.None))
            .Returns(false);
        var service = mock.CreateInstance<RedisCache>();

        var response = await service.GetAsync(Key, functionMock.Object);
        Assert.That(response, Is.EqualTo(Value));
    }
    
    [Test]
    public void GetAsync_BadJson_JsonReaderExceptionThrown()
    {
        var mock = new AutoMocker();
        var databaseMock = mock.GetMock<IDatabase>();
        databaseMock
            .Setup(x => x.StringGetAsync(Key, CommandFlags.None))
            .ReturnsAsync("BAD JSON PAYLOAD");
        var service = mock.CreateInstance<RedisCache>();

        Assert.ThrowsAsync<JsonReaderException>(() => service.GetAsync<int>(Key));
    }
    
    [Test]
    public void Set_ValueSet_TrueReturned()
    {
        var mock = new AutoMocker();
        var databaseMock = mock.GetMock<IDatabase>();
        databaseMock
            .Setup(x => x.StringSet(Key, Value.ToString(), redisCacheMinutes, false, When.Always, CommandFlags.None))
            .Returns(true);
        var service = mock.CreateInstance<RedisCache>();

        var response = service.Set(Key, Value, redisCacheMinutes);
        Assert.That(response, Is.True);
    }
    
    [Test]
    public void Set_ValueNotSet_FalseReturned()
    {
        var mock = new AutoMocker();
        var databaseMock = mock.GetMock<IDatabase>();
        databaseMock
            .Setup(x => x.StringSet(Key, Value.ToString(), TimeSpan.FromMinutes(60), false, When.Always, CommandFlags.None))
            .Returns(false);
        var service = mock.CreateInstance<RedisCache>();

        var response = service.Set(Key, Value);
        Assert.That(response, Is.False);
    }
    
    [Test]
    public async Task SetAsync_ValueSet_TrueReturned()
    {
        var mock = new AutoMocker();
        var databaseMock = mock.GetMock<IDatabase>();
        databaseMock
            .Setup(x => x.StringSetAsync(Key, Value.ToString(), redisCacheMinutes, false, When.Always, CommandFlags.None))
            .ReturnsAsync(true);
        var service = mock.CreateInstance<RedisCache>();

        var response = await service.SetAsync(Key, Value, redisCacheMinutes);
        Assert.That(response, Is.True);
    }
    
    [Test]
    public async Task SetAsync_ValueNotSet_FalseReturned()
    {
        var mock = new AutoMocker();
        var databaseMock = mock.GetMock<IDatabase>();
        databaseMock
            .Setup(x => x.StringSetAsync(Key, Value.ToString(), TimeSpan.FromMinutes(60), false, When.Always, CommandFlags.None))
            .ReturnsAsync(false);
        var service = mock.CreateInstance<RedisCache>();

        var response = await service.SetAsync(Key, Value);
        Assert.That(response, Is.False);
    }

    [Test]
    public void Delete_ValueDeleted_TrueReturned()
    {
        var mock = new AutoMocker();
        var databaseMock = mock.GetMock<IDatabase>();
        databaseMock.Setup(x => x.StringGetDelete(Key, CommandFlags.None)).Returns(Value);
        var service = mock.CreateInstance<RedisCache>();

        var response = service.Delete(Key);
        Assert.That(response, Is.True);
    }
    
    [Test]
    public void Delete_ValueNotDeleted_FalseReturned()
    {
        var mock = new AutoMocker();
        var databaseMock = mock.GetMock<IDatabase>();
        databaseMock.Setup(x => x.StringGetDelete(Key, CommandFlags.None)).Returns(new RedisValue());
        var service = mock.CreateInstance<RedisCache>();

        var response = service.Delete(Key);
        Assert.That(response, Is.False);
    }
    
    [Test]
    public async Task DeleteAsync_ValueDeleted_TrueReturned()
    {
        var mock = new AutoMocker();
        var databaseMock = mock.GetMock<IDatabase>();
        databaseMock
            .Setup(x => x.StringGetDeleteAsync(Key, CommandFlags.None))
            .ReturnsAsync(Value);
        var service = mock.CreateInstance<RedisCache>();

        var response = await service.DeleteAsync(Key);
        Assert.That(response, Is.True);
    }
    
    [Test]
    public async Task DeleteAsync_ValueNotDeleted_FalseReturned()
    {
        var mock = new AutoMocker();
        var databaseMock = mock.GetMock<IDatabase>();
        databaseMock
            .Setup(x => x.StringGetDeleteAsync(Key, CommandFlags.None))
            .ReturnsAsync(new RedisValue());
        var service = mock.CreateInstance<RedisCache>();

        var response = await service.DeleteAsync(Key);
        Assert.That(response, Is.False);
    }

    private const string Key = "My Super Fun Key!";
    private const int Value = 42;
    private readonly TimeSpan redisCacheMinutes = TimeSpan.FromMinutes(15);
}