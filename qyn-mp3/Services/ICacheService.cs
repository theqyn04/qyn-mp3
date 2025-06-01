namespace qyn_mp3.Services
{
    public interface ICacheService
    {
        void Set<T>(string key, T value, TimeSpan expiry);
        T Get<T>(string key);
        bool TryGetValue<T>(string key, out T value);
        void Remove(string key);
    }

}
