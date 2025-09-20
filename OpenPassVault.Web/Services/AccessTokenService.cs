using Microsoft.IdentityModel.Tokens;
using OpenPassVault.Web.Services.Interfaces;

namespace OpenPassVault.Web.Services;

public class AccessTokenService(IMemoryStorageService memoryStorageService) : IAccessTokenService
{
    private const string OpenPassVaultApiTokenKey = "OpenPassVault.API";
    
    public string? GetToken()
    {
        return memoryStorageService.GetItem<string>(OpenPassVaultApiTokenKey);
    }

    public void SaveToken(string token)
    {
        memoryStorageService.SetItem(OpenPassVaultApiTokenKey, token);
    }
    
    public void RemoveToken()
    {
        memoryStorageService.RemoveItem(OpenPassVaultApiTokenKey);
    }
}