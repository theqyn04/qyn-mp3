using Microsoft.Extensions.Caching.Memory;

namespace qyn_mp3.Services
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void Set<T>(string key, T value, TimeSpan expiry)
        {
            _cache.Set(key, value, expiry);
        }

        public T Get<T>(string key)
        {
            return _cache.Get<T>(key);
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            return _cache.TryGetValue(key, out value);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }

}
