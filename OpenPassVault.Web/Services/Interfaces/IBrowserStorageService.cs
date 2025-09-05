namespace OpenPassVault.Web.Services.Interfaces;

public interface IBrowserStorageService
{
    Task<T?> GetItem<T>(string key);
    Task SetItem<T>(string key, T value);
    Task RemoveItem(string key);
}