namespace OpenPassVault.Web.Services.Interfaces;

public interface IMemoryStorageService
{
    T? GetItem<T>(string key);
    void SetItem<T>(string key, T value);
    void RemoveItem(string key);
}