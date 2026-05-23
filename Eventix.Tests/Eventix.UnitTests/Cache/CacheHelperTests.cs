using Eventix.Api.Helpers;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Eventix.UnitTests.Cache;

public class CacheHelperTests
{
    private readonly IDistributedCache _cache =
        new MemoryDistributedCache(
            Microsoft.Extensions.Options.Options.Create(
                new MemoryDistributedCacheOptions()));

    [Fact]
    public async Task GetOrSetAsync_When_Cache_Empty_Should_Call_GetData()
    {
        var called = false;

        var result = await CacheHelper.GetOrSetAsync(
            _cache,
            "test-key",
            () =>
            {
                called = true;
                return Task.FromResult("hello");
            },
            TimeSpan.FromMinutes(5));

        Assert.True(called);
        Assert.Equal("hello", result);
    }

    [Fact]
    public async Task GetOrSetAsync_When_Cache_Has_Value_Should_Not_Call_GetData()
    {
        await CacheHelper.GetOrSetAsync(
            _cache,
            "cached-key",
            () => Task.FromResult("first-value"),
            TimeSpan.FromMinutes(5));

        var calledAgain = false;

        var result = await CacheHelper.GetOrSetAsync(
            _cache,
            "cached-key",
            () =>
            {
                calledAgain = true;
                return Task.FromResult("second-value");
            },
            TimeSpan.FromMinutes(5));

        Assert.False(calledAgain);
        Assert.Equal("first-value", result);
    }

    [Fact]
    public async Task RemoveAsync_Should_Remove_Cache_Key()
    {
        await CacheHelper.GetOrSetAsync(
            _cache,
            "remove-key",
            () => Task.FromResult("value"),
            TimeSpan.FromMinutes(5));

        await CacheHelper.RemoveAsync(_cache, "remove-key");

        var result = await _cache.GetStringAsync("remove-key");

        Assert.Null(result);
    }
}