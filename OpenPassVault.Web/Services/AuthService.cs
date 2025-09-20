using System.Security.Claims;
using OpenPassVault.Shared.Auth;
using OpenPassVault.Shared.Crypto.Interfaces;
using OpenPassVault.Shared.DTO;
using OpenPassVault.Web.Helpers;
using OpenPassVault.Web.Models;
using OpenPassVault.Web.Services.Exceptions;
using OpenPassVault.Web.Services.Interfaces;

namespace OpenPassVault.Web.Services;

public class AuthService(
    IHttpApiService httpApiService, 
    IPasswordHasher passwordHasher,
    IAccessTokenService accessTokenService) : IAuthService
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
        
        accessTokenService.SaveToken(response.Token);
        var principal = GetClaimsPrincipalFromToken();
        var masterPasswordHash = principal?.Claims.First(
            c => c.Type == JwtClaimType.TokenMasterPasswordHashClaimType).Value;

        if (string.IsNullOrEmpty(masterPasswordHash))
            throw new WrongMasterPasswordException();
        
        var isValid = await passwordHasher.VerifyPassword(
            hashedPassword: masterPasswordHash, 
            loginViewModel.MasterPassword);
        
        return isValid ? principal : throw new WrongMasterPasswordException();
    }

    public ClaimsPrincipal? GetClaimsPrincipalFromToken()
    {
        var token = accessTokenService.GetToken();
       
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
    
    public void LogoutAsync()
    {
        accessTokenService.RemoveToken();
    }
}
