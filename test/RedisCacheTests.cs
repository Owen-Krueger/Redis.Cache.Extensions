using Moq;
using Moq.AutoMock;
using StackExchange.Redis;

namespace Redis.Sidecar.Cache.Tests;

public class RedisCacheTests
{
    [Test]
    public async Task GetAsync_ValueFound_ValueReturned()
    {
        var mock = new AutoMocker();
        var databaseMock = mock.GetMock<IDatabase>();
        databaseMock.Setup(x => x.StringGetAsync(Key, CommandFlags.None)).ReturnsAsync(Value);
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
        databaseMock.Setup(x => x.StringGetAsync(Key, CommandFlags.None)).ReturnsAsync(new RedisValue());
        var functionMock = new Mock<Func<Task<int>>>();
        functionMock.Setup(x => x.Invoke()).ReturnsAsync(Value);
        var service = mock.CreateInstance<RedisCache>();

        var response = await service.GetAsync(Key, functionMock.Object, redisCacheMinutes);
        Assert.That(response, Is.EqualTo(Value));
        databaseMock.Verify(x => x.StringSet(Key, It.IsAny<RedisValue>(), redisCacheMinutes, false, When.Always, CommandFlags.None), Times.Once);
    }

    [Test]
    public async Task GetAsync_ValueQueryFailed_ValueNotSaved()
    {
        var mock = new AutoMocker();
        var databaseMock = mock.GetMock<IDatabase>();
        databaseMock.Setup(x => x.StringGetAsync(Key, CommandFlags.None)).ReturnsAsync(new RedisValue());
        var functionMock = new Mock<Func<Task<int?>>>();
        functionMock.Setup(x => x.Invoke()).ReturnsAsync((int?)null);
        var service = mock.CreateInstance<RedisCache>();

        var response = await service.GetAsync(Key, functionMock.Object);
        Assert.That(response, Is.Null);
        databaseMock.Verify(x => x.StringSet(Key, It.IsAny<RedisValue>(), redisCacheMinutes, false, When.Always, CommandFlags.None), Times.Never);
    }

    [Test]
    public async Task GetAsync_ValueNotSaved_ValueReturned()
    {
        var mock = new AutoMocker();
        var databaseMock = mock.GetMock<IDatabase>();
        databaseMock.Setup(x => x.StringGetAsync(Key, CommandFlags.None)).ReturnsAsync(new RedisValue());
        var functionMock = new Mock<Func<Task<int>>>();
        functionMock.Setup(x => x.Invoke()).ReturnsAsync(Value);
        databaseMock.Setup(x => x.StringSet(Key, It.IsAny<RedisValue>(), redisCacheMinutes, false, When.Always, CommandFlags.None)).Returns(false);
        var service = mock.CreateInstance<RedisCache>();

        var response = await service.GetAsync(Key, functionMock.Object);
        Assert.That(response, Is.EqualTo(Value));
    }

    private const string Key = "My Super Fun Key!";
    private const int Value = 42;
    private readonly TimeSpan redisCacheMinutes = TimeSpan.FromMinutes(15);
}