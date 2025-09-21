namespace OpenPassVault.Web.Services.Interfaces;

public interface IMasterPasswordStorage
{
    Task SetMasterPassword(string masterPassword);
    Task<string?> GetMasterPassword();
    Task ClearMasterPassword();
}