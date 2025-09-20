using System.Diagnostics;
using System.Security.Claims;
using Blazored.SessionStorage;
using OpenPassVault.Shared.Crypto.Interfaces;
using OpenPassVault.Shared.DTO;
using OpenPassVault.Web.Helpers;
using OpenPassVault.Web.Models;
using OpenPassVault.Web.Services.Interfaces;

namespace OpenPassVault.Web.Services;

public class AuthService(
    IHttpApiService httpApiService, 
    ISessionStorageService sessionStorageService,
    IMemoryStorageService memoryStorageService,
    IPasswordHasher passwordHasher,
    ILogger<AuthService> logger) : IAuthService
{
    private const string AuthBaseUrl = "auth";
    private const string LoginUrl = $"{AuthBaseUrl}/login";
    private const string RegisterUrl = $"{AuthBaseUrl}/register";

    private const string ApiTokenKey = "OpenPassVault.API";
    
    public async Task<ClaimsPrincipal?> LoginAsync(LoginRequest loginRequest)
    {
        var response = await httpApiService.PostAsync<TokenResponse>(LoginUrl, loginRequest);
        if (response == null)
            return null;
        logger.LogInformation($"Storing token in session storage... {response.Token}");
        memoryStorageService.SetItem(ApiTokenKey, response.Token);
        await sessionStorageService.SetItemAsStringAsync(ApiTokenKey, response.Token);
        logger.LogInformation(await sessionStorageService.GetItemAsStringAsync(ApiTokenKey));
        return await GetClaimsPrincipalFromToken();
    }

    public async Task<ClaimsPrincipal?> GetClaimsPrincipalFromToken()
    {
        var token = memoryStorageService.GetItem<string>(ApiTokenKey);
        logger.LogInformation(token); // TODO: Remove in production
       
        if (string.IsNullOrEmpty(token))
            return null;
        
        var principal = JwtParser.ToClaimsPrincipal(token);
        logger.LogInformation($"Name {principal.Identity?.Name}");
        logger.LogInformation($"Authenticated {principal.Identity?.IsAuthenticated}");
        logger.LogInformation($"Name {principal.Claims.FirstOrDefault(x => x.Type == "mp_hash")?.Value}");
        return principal;
    }

    public async Task RegisterAsync(RegisterViewModel registerViewModel)
    {
        var masterPasswordHash = await passwordHasher.HashPassword(registerViewModel.MasterPassword);
        var request = new RegisterRequest
        {
            Email = registerViewModel.Email!,
            Password = registerViewModel.Password!,
            ConfirmPassword = registerViewModel.ConfirmPassword!,
            MasterPasswordHash = masterPasswordHash
        };
        
        await httpApiService.PostAsync<string>(RegisterUrl, request);
    }   
    
    public void LogoutAsync()
    {
        memoryStorageService.RemoveItem(ApiTokenKey);
    }
}
