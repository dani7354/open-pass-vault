using System.Security.Claims;
using OpenPassVault.Shared.Auth;
using OpenPassVault.Shared.Crypto.Interfaces;
using OpenPassVault.Shared.DTO;
using OpenPassVault.Shared.Helpers;
using OpenPassVault.Web.Models;
using OpenPassVault.Web.Services.Exceptions;
using OpenPassVault.Web.Services.Interfaces;

namespace OpenPassVault.Web.Services;

public class AuthService(
    IHttpApiService httpApiService, 
    IPasswordHasher passwordHasher,
    IAccessTokenStorage accessTokenStorage,
    IMasterPasswordStorage masterPasswordStorage) : IAuthService
{
    private const string AuthBaseUrl = "auth";
    private const string LoginUrl = $"{AuthBaseUrl}/login";
    private const string RegisterUrl = $"{AuthBaseUrl}/register";

    
    public async Task<ClaimsPrincipal?> LoginAsync(LoginViewModel loginViewModel)
    {
        var loginRequest = new LoginRequest
        {
            Email = loginViewModel.Email,
            Password = loginViewModel.Password
        };
        
        var response = await httpApiService.PostAsync<TokenResponse>(LoginUrl, loginRequest);
        if (response == null)
            throw new AuthenticationException();

        var principal = JwtParser.ToClaimsPrincipal(response.Token);
        var masterPasswordHash = principal?.Claims.First(
            c => c.Type == JwtClaimType.TokenMasterPasswordHashClaimType).Value;

        if (string.IsNullOrEmpty(masterPasswordHash))
            throw new WrongMasterPasswordException();
        
        var isValid = await passwordHasher.VerifyPassword(
            hashedPassword: masterPasswordHash, 
            loginViewModel.MasterPassword);

        if (!isValid) throw new WrongMasterPasswordException();
        
        await masterPasswordStorage.SetMasterPassword(masterPasswordHash);
        await accessTokenStorage.SetToken(response.Token);
            
        return principal;
    }

    public async Task<ClaimsPrincipal?> GetClaimsPrincipalFromToken()
    {
        var token = await accessTokenStorage.GetToken();
       
        return string.IsNullOrEmpty(token) ? null : JwtParser.ToClaimsPrincipal(token);
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
    
    public async Task LogoutAsync()
    {
        await accessTokenStorage.ClearToken();
        await masterPasswordStorage.ClearMasterPassword();
    }
}
