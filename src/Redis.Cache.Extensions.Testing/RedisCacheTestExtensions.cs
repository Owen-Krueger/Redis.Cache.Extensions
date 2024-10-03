using Moq;
using Moq.Language.Flow;
// ReSharper disable InvalidXmlDocComment

namespace Redis.Cache.Extensions.Testing;

/// <summary>
/// Extensions for <see cref="IRedisCache"/> mocks that can be utilized in tests.
/// </summary>
public static class RedisCacheTestExtensions
{
    /// <summary>
    /// Specifies a setup on the mocked <see cref="IRedisCache"/> type for a call to the `Get` method.
    /// </summary>
    /// <param name="redisCacheMock">A mocked <see cref="IRedisCache"/>.</param>
    /// <param name="key">Optional. The identifier for the value to get from the cache expected to be utilized.</param>
    /// <typeparam name="T">The type of object stored in the cache.</typeparam>
    /// <example>
    ///     <code>
    ///         var redisMock = new Mock<IRedisCache>();
    ///         redisMock.SetupRedisGet<bool>("Key")
    ///             .Returns(true);
    ///     </code>
    /// </example>
    public static ISetup<IRedisCache, T?> SetupRedisGet<T>(this Mock<IRedisCache> redisCacheMock, string? key = null)
    {
        return key is null ? 
            redisCacheMock.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<Func<T>>(), It.IsAny<Func<T, bool>>())) : 
            redisCacheMock.Setup(x => x.Get(key, It.IsAny<Func<T>>(), It.IsAny<Func<T, bool>>()));
    }

    /// <summary>
    /// Specifies a setup on the mocked <see cref="IRedisCache"/> type for a call to the `GetAsync` method.
    /// </summary>
    /// <param name="redisCacheMock">A mocked <see cref="IRedisCache"/>.</param>
    /// <param name="key">Optional. The identifier for the value to get from the cache expected to be utilized.</param>
    /// <typeparam name="T">The type of object stored in the cache.</typeparam>
    /// <example>
    ///     <code>
    ///         var redisMock = new Mock<IRedisCache>();
    ///         redisMock.SetupRedisGetAsync<bool>("Key")
    ///             .ReturnsAsync(true);
    ///     </code>
    /// </example>
    public static ISetup<IRedisCache, Task<T?>> SetupRedisGetAsync<T>(this Mock<IRedisCache> redisCacheMock,
        string? key = null)
    {
        return key is null ? 
            redisCacheMock.Setup(x => x.GetAsync( It.IsAny<string>(), It.IsAny<Func<Task<T>>>(), It.IsAny<Func<T, bool>>())) :
            redisCacheMock.Setup(x => x.GetAsync( key, It.IsAny<Func<Task<T>>>(), It.IsAny<Func<T, bool>>()));
    }

    /// <summary>
    /// Specifies that the inner function in the <see cref="IRedisCache"/> type's `Get` method will be invoked to
    /// return the value.
    /// </summary>
    /// <param name="redisCacheMock">A setup for a <see cref="IRedisCache"/> `Get` call.</param>
    /// <typeparam name="T">The type of object stored in the cache.</typeparam>
    /// <example>
    ///     <code>
    ///         var redisMock = new Mock<IRedisCache>();
    ///         redisMock.SetupRedisGet<bool>("Key")
    ///             .ReturnsFromInnerFunction();
    ///     </code>
    /// </example>
    public static IReturnsResult<IRedisCache> ReturnFromInnerFunction<T>(this ISetup<IRedisCache, T?> setup)
        => setup.Returns((string _, Func<T?> fun, Func<T?, bool> _) => fun());
    
    /// <summary>
    /// Specifies that the inner function in the <see cref="IRedisCache"/> type's `GetAsync` method will be invoked to
    /// return the value.
    /// </summary>
    /// <param name="redisCacheMock">A setup for a <see cref="IRedisCache"/> `GetAsync` call.</param>
    /// <typeparam name="T">The type of object stored in the cache.</typeparam>
    /// <example>
    ///     <code>
    ///         var redisMock = new Mock<IRedisCache>();
    ///         redisMock.SetupRedisGetAsync<bool>("Key")
    ///             .ReturnsFromInnerFunction();
    ///     </code>
    /// </example>
    public static IReturnsResult<IRedisCache> ReturnFromInnerFunction<T>(this ISetup<IRedisCache, Task<T?>> setup)
        => setup.Returns((string _, Func<Task<T?>> fun, Func<T?, bool> _) => fun());
}