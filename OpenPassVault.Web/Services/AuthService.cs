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
    private const string DeleteUserUrl = $"{AuthBaseUrl}/delete";
    private const string UserInfoUrl = $"{AuthBaseUrl}/user-info";
    
    private const string CaptchaCodeParam = "captchaCode";
    private const string CaptchaHmacParam = "captchaHmac";

    
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
        var masterPasswordHash = principal.Claims.First(
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

    public async Task<EditUserViewModel> CreateEditUserViewModel()
    {
        var userInfo = await httpApiService.GetAsync<UserInfoResponse>(UserInfoUrl);
        if (userInfo == null)
            throw new ApiRequestUnauthorizedException("Failed to get user info.");
        
        var newCaptcha = await captchaApiService.GetNewCaptcha();
        var captchaImageSource = FormatImageSource(newCaptcha.CaptchaImageBase64);

        var viewModel = new EditUserViewModel
        {
            Id = userInfo.Id,
            Email = userInfo.Email,
            CaptchaHmac = newCaptcha.CaptchaHmac,
            CaptchaImageSrc = captchaImageSource
        };

        return viewModel;
    }

    public async Task UpdateUserInfo(EditUserViewModel editUserViewModel)
    {
        var request = new UpdateUserRequest
        {
            Email = editUserViewModel.Email,
            NewPassword = editUserViewModel.NewPassword,
            ConfirmNewPassword = editUserViewModel.ConfirmNewPassword,
            CaptchaCode = editUserViewModel.CaptchaCode,
            CaptchaHmac = editUserViewModel.CaptchaHmac
        };
        
        await httpApiService.PutAsync<string>(UserInfoUrl, request);
    }
    
    public async Task<DeleteUserViewModel> CreateDeleteUserViewModel()
    {
        var userInfo = await httpApiService.GetAsync<UserInfoResponse>(UserInfoUrl);
        if (userInfo == null)
            throw new ApiRequestUnauthorizedException("Failed to get user info.");
        
        var newCaptcha = await captchaApiService.GetNewCaptcha();
        var captchaImageSource = FormatImageSource(newCaptcha.CaptchaImageBase64);

        var deleteUserViewModel = new DeleteUserViewModel
        {
            UserId = userInfo.Id,
            CaptchaHmac = newCaptcha.CaptchaHmac,
            CaptchaImageSrc = captchaImageSource
        };

        return deleteUserViewModel;
    }

    public async Task DeleteUser(DeleteUserViewModel deleteUserViewModel)
    {
        var url = $"{DeleteUserUrl}?{CaptchaCodeParam}={Uri.EscapeDataString(deleteUserViewModel.CaptchaCode)}";
        url += $"&{CaptchaHmacParam}={Uri.EscapeDataString(deleteUserViewModel.CaptchaHmac)}";
        
        await httpApiService.DeleteAsync(url);
        await LogoutAsync();
    }

    public async Task LogoutAsync()
    {
        await accessTokenStorage.ClearToken();
        await masterPasswordStorage.ClearMasterPassword();
    }
    
    private static string FormatImageSource(string base64) => $"data:image/png;base64,{base64}";
}
