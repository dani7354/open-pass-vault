using System.Security.Claims;
using OpenPassVault.Shared.Crypto.Interfaces;
using OpenPassVault.Shared.DTO;
using OpenPassVault.Web.Helpers;
using OpenPassVault.Web.Models;
using OpenPassVault.Web.Services.Interfaces;

namespace OpenPassVault.Web.Services;

public class AuthService(
    IHttpApiService httpApiService, 
    IBrowserStorageService browserStorageService,
    IPasswordHasher passwordHasher) : IAuthService
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

    public async Task<ClaimsPrincipal?> GetClaimsPrincipalFromToken()
    {
        var token = await browserStorageService.GetItem<string>(ApiTokenKey);
        return token == null ? null : JwtParser.ToClaimsPrincipal(token);
    }

    public async Task<bool> RegisterAsync(RegisterViewModel registerViewModel)
    {
        if (string.IsNullOrEmpty(registerViewModel.MasterPassword))
            return false;
        
        var masterPasswordHash = await passwordHasher.HashPassword(registerViewModel.MasterPassword);
        var request = new RegisterRequest()
        {
            Email = registerViewModel.Email,
            Name = registerViewModel.Name,
            Password = registerViewModel.Password,
            ConfirmPassword = registerViewModel.ConfirmPassword,
            MasterPasswordHash = masterPasswordHash
        };
        
        var response = await httpApiService.PostAsync<string>(RegisterUrl, request);
        return response != null;
    }
    
    public async Task LogoutAsync()
    {
        await browserStorageService.RemoveItem(ApiTokenKey);
    }
}
