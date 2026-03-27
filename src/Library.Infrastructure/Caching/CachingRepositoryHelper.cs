using Microsoft.Extensions.Caching.Memory;

namespace Library.Infrastructure.Caching;

internal static class CachingRepositoryHelper
{
    internal static async Task<IEnumerable<T>> GetAllAsync<T>(
        IMemoryCache cache,
        object listKey,
        Func<CancellationToken, Task<IEnumerable<T>>> load,
        TimeSpan ttl,
        CancellationToken ct = default)
    {
        if (cache.TryGetValue(listKey, out IEnumerable<T>? cached))
            return cached!;

        var result = await load(ct);
        cache.Set(listKey, result, ttl);
        return result;
    }

    internal static async Task<T?> GetByIdAsync<T>(
        IMemoryCache cache,
        object itemKey,
        Func<CancellationToken, Task<T?>> load,
        TimeSpan ttl,
        CancellationToken ct = default) where T : class
    {
        if (cache.TryGetValue(itemKey, out T? cached))
            return cached;

        var result = await load(ct);
        if (result is not null)
            cache.Set(itemKey, result, ttl);
        return result;
    }

    internal static void InvalidateList(IMemoryCache cache, object listKey)
        => cache.Remove(listKey);

    internal static void InvalidateListAndItem(IMemoryCache cache, object listKey, object itemKey)
    {
        cache.Remove(listKey);
        cache.Remove(itemKey);
    }
}
