using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SocialNetwork.DAL.Options;

namespace SocialNetwork.DAL.Services;

public class CacheService<T>
{
    private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions
    {
        SizeLimit = 1000,
        ExpirationScanFrequency = TimeSpan.FromMinutes(5)
    });

    private readonly MemoryCacheEntryOptions _entryOptions;

    private readonly SemaphoreSlim _cacheLock = new(1);


    public CacheService(IOptions<CacheOptions> options)
    {
        _entryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(options.Value.CacheTime);
    }

    public async Task<T?> GetOrSetAsync(string key, Func<CancellationToken, Task<T>> getDataFunc,
        CancellationToken cancellationToken, SocialNetworkDbContext? socialNetworkDbContext = null)
    {
        if (_cache.TryGetValue(key, out T? result))
        {
            socialNetworkDbContext.Attach(result!);
            return result;
        }

        await _cacheLock.WaitAsync(cancellationToken);
        try
        {
            T? newValue = await getDataFunc(cancellationToken);
            _cache.Set(key, newValue, _entryOptions.SetSize(1));
            return newValue;
        }
        finally
        {
            _cacheLock.Release();
        }
    }

    public async Task<T?> UpdateAsync(string key, Func<CancellationToken, Task<T>> getDataFunc,
        CancellationToken cancellationToken)
    {
        await _cacheLock.WaitAsync(cancellationToken);
        
        T? newValue = await getDataFunc(cancellationToken);
        _cache.Set(key, newValue, _entryOptions.SetSize(1));
        
        _cacheLock.Release();
        
        return newValue;
    }

    public async Task RemoveFromCacheAsync(string key, CancellationToken cancellationToken)
    {
        if (!_cache.TryGetValue(key, out T? _))
            return;

        await _cacheLock.WaitAsync(cancellationToken);

        _cache.Remove(key);

        _cacheLock.Release();
    }
}