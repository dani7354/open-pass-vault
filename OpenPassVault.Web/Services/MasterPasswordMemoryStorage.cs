using OpenPassVault.Web.Services.Interfaces;

namespace OpenPassVault.Web.Services;

public class MasterPasswordMemoryStorage(IMemoryStorageService memoryStorageService) 
    : AbstractFieldMemoryStorage<string>(memoryStorageService, MasterPasswordKey), IMasterPasswordStorage
{
    private const string MasterPasswordKey = "OpenPassVault.MasterPassword";

    public Task SetMasterPassword(string masterPassword)
    {
        Save(masterPassword);
        return Task.CompletedTask;
    }

    public Task<string?> GetMasterPassword()
    {
        return Task.FromResult(Get());
    }

    public Task ClearMasterPassword()
    {
        Remove();
        return Task.CompletedTask;
    }
}