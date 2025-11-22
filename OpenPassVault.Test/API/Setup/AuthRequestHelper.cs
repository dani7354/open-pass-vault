using System.Net;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Net.Http.Headers;
using OpenPassVault.API.Helpers;
using OpenPassVault.Shared.Crypto;
using OpenPassVault.Shared.DTO;
using ApiConstants = OpenPassVault.API.Shared.Constants;

namespace OpenPassVault.Test.API.Setup;

public static class AuthRequestHelper
{
    private const string TestUserEmail = "a-mail@gmail.com";
    private const string TestUserPassword = "StrongPassword123!";
    private const string TestUserMasterPasswordHash = "MasterPasswordHashExample";
    
    public static async Task RegisterUserAndSetupAuthenticatedClient(
        HttpClient client, 
        string email = TestUserEmail)
    {
        await RegisterValidTestUser(client, email: email);
        var (csrfTokenCookie, tokenResponse) = await LoginValidTestUser(client, email: email);
        
        client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme, 
                tokenResponse.Token);
        
        client.DefaultRequestHeaders.Add(HeaderNames.Cookie, csrfTokenCookie);
    }
    
    public static async Task RegisterValidTestUser(
        HttpClient client,
        string email = TestUserEmail,
        string password = TestUserPassword)
    {
        var registerRequestPayload = await GetRegisterRequestPayload(
            email: email, 
            password: password, 
            confirmPassword: password);

        var response = await client.PostAsync(Endpoint.AuthRegisterEndpoint, registerRequestPayload);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    public static async Task<(string csrfTokenCookie, TokenResponse)> LoginValidTestUser(
        HttpClient client,
        string email = TestUserEmail,
        string password = TestUserPassword)
    {
        var loginRequest = new LoginRequest
        {
            Email = email,
            Password = password
        };
        
        var loginRequestPayload = HttpContentHelper.CreateStringContent(loginRequest);
        var loginResponse = await client.PostAsync(Endpoint.AuthLoginEndpoint, loginRequestPayload);

        string? csrfTokenCookie = null;
        if (loginResponse.Headers.TryGetValues("Set-Cookie", out var cookieValues))
            csrfTokenCookie = cookieValues
                .FirstOrDefault(c => c.StartsWith(ApiConstants.Cookies.CsrfToken));

        Assert.NotNull(csrfTokenCookie);
        
        var tokenResponse = await loginResponse.Content.ReadFromJsonAsync<TokenResponse>();
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        Assert.True(!string.IsNullOrEmpty(tokenResponse?.Token));

        return (csrfTokenCookie, tokenResponse);
    }

    public static async Task<StringContent> GetRegisterRequestPayload(
        string email = TestUserEmail,
        string password = TestUserPassword,
        string confirmPassword = TestUserPassword,
        string masterPasswordHash = TestUserMasterPasswordHash)
    {
        var captchaCode = "xy45xxswr";
        var key = EnvironmentHelper.GetCsrfTokenKey();
        var hmacService = new HmacService(key);
        var captchaHmac = await hmacService.CreateHmac(Encoding.UTF8.GetBytes(captchaCode));
        
        var userRegisterRequest = new RegisterRequest
        {
            Email = email,
            Password = password,
            ConfirmPassword = confirmPassword,
            MasterPasswordHash = masterPasswordHash,
            CaptchaCode = captchaCode,
            CaptchaHmac = captchaHmac
        };
        
        return HttpContentHelper.CreateStringContent(userRegisterRequest);
    }
}