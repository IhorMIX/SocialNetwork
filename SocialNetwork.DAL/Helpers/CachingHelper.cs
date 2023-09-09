using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading;
using System.Threading.Tasks;

public class CachingHelper<T>
{
    private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions
    {
        SizeLimit = 1000,
        ExpirationScanFrequency = TimeSpan.FromMinutes(5)
    });

    private readonly MemoryCacheEntryOptions _entryOptions = new MemoryCacheEntryOptions()
        .SetSlidingExpiration(TimeSpan.FromSeconds(5))
        .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

    private readonly SemaphoreSlim _cacheLock = new SemaphoreSlim(1);

    public async Task<T> GetOrUpdate(string key, Func<T, CancellationToken, Task<T>> getDataFunc, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(key, out T result))
            return result;

        await _cacheLock.WaitAsync(cancellationToken);
        try
        {
            if (_cache.TryGetValue(key, out T cachedResult))
                return cachedResult;

            T newValue = await getDataFunc(key, cancellationToken);
            _cache.Set(key, newValue, _entryOptions.SetSize(1));
            return newValue;
        }
        finally
        {
            _cacheLock.Release();
        }
        
    }
}


