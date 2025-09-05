using OpenPassVault.Shared.DTO;
using OpenPassVault.Web.Services.Interfaces;

namespace OpenPassVault.Web.Services;

public class AuthService(IHttpApiService httpApiService, IBrowserStorageService browserStorageService) : IAuthService
{
    private const string AuthBaseUrl = "/auth";
    private const string LoginUrl = $"{AuthBaseUrl}/login";
    private const string RegisterUrl = $"{AuthBaseUrl}/register";

    private const string ApiTokenKey = "OpenPassVault.API";

    public async Task<bool> LoginAsync(LoginRequest loginRequest)
    {
        var response = await httpApiService.PostAsync<TokenResponse>(LoginUrl, loginRequest);
        if (response == null)
            return false;
        
        await browserStorageService.SetItem(ApiTokenKey, response.Token);
        return true;
    }

    public Task<string?> GetPersistedTokenAsync()
    {
        return browserStorageService.GetItem<string>(ApiTokenKey);
    }

    public async Task<bool> RegisterAsync(RegisterRequest registerRequest)
    {
        var response = await httpApiService.PostAsync<string>(RegisterUrl, registerRequest);
        return response != null;
    }
    
    public async Task LogoutAsync()
    {
        await browserStorageService.RemoveItem(ApiTokenKey);
    }
}
