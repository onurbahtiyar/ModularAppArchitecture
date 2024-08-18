using Microsoft.Extensions.Caching.Memory;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _cache;

    public CacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public T Get<T>(string key)
    {
        _cache.TryGetValue(key, out T value);
        return value;
    }

    public void Set<T>(string key, T value, TimeSpan cacheDuration)
    {
        _cache.Set(key, value, cacheDuration);
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
    }
}