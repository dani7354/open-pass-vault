using System.Data;
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
    ICaptchaApiService captchaApiService,
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

    public async Task<RegisterViewModel> CreateRegisterViewModel()
    {
        var newCaptcha = await captchaApiService.GetNewCaptcha();
        var captchaImageSource = FormatImageSource(newCaptcha.CaptchaImageBase64);

        var viewModel = new RegisterViewModel
        {
            CaptchaHmac = newCaptcha.CaptchaHmac,
            CaptchaImageSrc = captchaImageSource
        };

        return viewModel;
    }
    
    public async Task<RegisterViewModel> RefreshRegisterViewModel(RegisterViewModel currentViewModel)
    {
        var newCaptcha = await captchaApiService.GetNewCaptcha();
        var captchaImageSource = FormatImageSource(newCaptcha.CaptchaImageBase64);
        
        var newViewModel = new RegisterViewModel
        {
            Email = currentViewModel.Email,
            Password = currentViewModel.Password,
            ConfirmPassword = currentViewModel.ConfirmPassword,
            MasterPassword = currentViewModel.MasterPassword,
            ConfirmMasterPassword = currentViewModel.ConfirmMasterPassword,
            CaptchaHmac = newCaptcha.CaptchaHmac,
            CaptchaImageSrc = captchaImageSource,
            CaptchaCode = string.Empty
        };

        return newViewModel;
    }

    public async Task RegisterAsync(RegisterViewModel registerViewModel)
    {
        var masterPasswordHash = await passwordHasher.HashPassword(registerViewModel.MasterPassword);
        var request = new RegisterRequest
        {
            Email = registerViewModel.Email,
            Password = registerViewModel.Password,
            ConfirmPassword = registerViewModel.ConfirmPassword,
            MasterPasswordHash = masterPasswordHash,
            CaptchaCode = registerViewModel.CaptchaCode,
            CaptchaHmac = registerViewModel.CaptchaHmac
        };
        
        await httpApiService.PostAsync<string>(RegisterUrl, request);
    }
    
    public async Task LogoutAsync()
    {
        await accessTokenStorage.ClearToken();
        await masterPasswordStorage.ClearMasterPassword();
    }
    
    private static string FormatImageSource(string base64) => $"data:image/png;base64,{base64}";
}
