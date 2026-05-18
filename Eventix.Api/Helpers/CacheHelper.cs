using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Eventix.API.Helpers;

public static class CacheHelper
{
    public static async Task<T?> GetOrSetAsync<T>(
        IDistributedCache cache,
        string key,
        Func<Task<T>> getData,
        TimeSpan expiration,
        CancellationToken ct = default)
    {
        var cached = await cache.GetStringAsync(key, ct);

        if (!string.IsNullOrWhiteSpace(cached))
        {
            return JsonSerializer.Deserialize<T>(cached);
        }

        var data = await getData();

        await cache.SetStringAsync(
            key,
            JsonSerializer.Serialize(data),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            },
            ct);

        return data;
    }

    public static async Task RemoveAsync(
        IDistributedCache cache,
        params string[] keys)
    {
        foreach (var key in keys)
        {
            await cache.RemoveAsync(key);
        }
    }
}