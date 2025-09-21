using Microsoft.IdentityModel.Tokens;
using OpenPassVault.Web.Services.Interfaces;

namespace OpenPassVault.Web.Services;

public class AccessTokenStorage(IMemoryStorageService memoryStorageService) 
    : AbstractFieldMemoryStorage<string>(memoryStorageService, AccessTokenKey), IAccessTokenStorage
{
    private const string AccessTokenKey = "OpenPassVault.Token";

    public Task SetToken(string token)
    {
        Save(token);
        return Task.CompletedTask;
    }
    
    public Task<string?> GetToken()
    {
        return Task.FromResult(Get());
    }
    
    public Task ClearToken()
    {
        Remove();
        return Task.CompletedTask;
    }
}