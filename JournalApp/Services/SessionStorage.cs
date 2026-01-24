using JournalApp.Services;
using Microsoft.Maui.Storage;

namespace JournalApp.Services
{
    public class SessionStorage : ISessionStorage
    {
        public async Task SetAsync(string key, string value)
        {
            await SecureStorage.Default.SetAsync(key, value);
        }

        public async Task<string?> GetAsync(string key)
        {
            return await SecureStorage.Default.GetAsync(key);
        }

        public void Remove(string key)
        {
            SecureStorage.Default.Remove(key);
        }
    }
}
