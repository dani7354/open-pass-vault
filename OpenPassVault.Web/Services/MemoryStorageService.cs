using OpenPassVault.Web.Services.Interfaces;

namespace OpenPassVault.Web.Services;

public class MemoryStorageService : IMemoryStorageService
{
    private static Dictionary<string, object> _memoryStorage = new();
    
    public T? GetItem<T>(string key)
    {
        return _memoryStorage.TryGetValue(key, out var value) ? (T?)value : default;
    }

    public void SetItem<T>(string key, T value)
    {
        if (value != null)
            _memoryStorage[key] = value;
    }

    public void RemoveItem(string key)
    {
        _memoryStorage.Remove(key);
    }
}