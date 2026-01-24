namespace JournalApp.Services
{
    public interface ISessionStorage
    {
        Task SetAsync(string key, string value);
        Task<string?> GetAsync(string key);
        void Remove(string key);
    }
}
