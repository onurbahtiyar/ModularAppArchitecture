public interface ICacheService
{
    T Get<T>(string key);

    void Set<T>(string key, T value, TimeSpan cacheDuration);

    void Remove(string key);
}