using OpenPassVault.Web.Services.Interfaces;

namespace OpenPassVault.Web.Services;

public abstract class AbstractFieldMemoryStorage<T>(IMemoryStorageService memoryStorageService, string itemKey)
{
    protected string? Get()
    {
        return memoryStorageService.GetItem<string>(itemKey);
    }

    protected void Save(T value)
    {
        memoryStorageService.SetItem(itemKey, value);
    }
    
    protected void Remove()
    {
        memoryStorageService.RemoveItem(itemKey);
    }
}