using System.Net;
using System.Net.Http.Json;
using OpenPassVault.Shared.DTO;
using OpenPassVault.Test.API.Setup;
using System.Text;
using OpenPassVault.API.Helpers;
using OpenPassVault.Shared.Crypto;

namespace OpenPassVault.Test.API.IntegrationTest;

public class AuthControllerTest : ControllerTestBase
{
    private const string TestUserEmail = "a-mail@gmail.com";
    private const string TestUserPassword = "StrongPassword123!";
    private const string TestUserMasterPasswordHash = "MasterPasswordHashExample";
    
    public static IEnumerable<object[]> EndpointsWithAuthentication =>
        new List<object[]>
        {
            new object[] { Endpoint.AuthUserInfoEndpoint, "GET" },
            new object[] { Endpoint.AuthDeleteEndpoint, "DELETE" },
        };
    
    #region Tests
    
    [Theory]
    [MemberData(nameof(EndpointsWithAuthentication))]
    public async Task EndpointsRequireAuthentication_NotAuthenticated_ReturnsUnauthorized(
        string endpoint, 
        string httpMethod)
    {
        var client = Factory.CreateClient();
        HttpResponseMessage response = httpMethod switch
        {
            "GET" => await client.GetAsync(endpoint),
            "DELETE" => await client.DeleteAsync(endpoint),
            _ => throw new NotSupportedException($"HTTP method {httpMethod} is not supported in this test.")
        };

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task Register_ValidInput_ReturnsSuccess()
    {
        var client = Factory.CreateClient();
        await RegisterValidTestUser(client);
    }

    [Fact]
    public async Task Login_ValidInput_ReturnsSuccess()
    {
        var client = Factory.CreateClient();
        await RegisterValidTestUser(client);
        var tokenResponse = await LoginValidTestUser(client);
        
        Assert.True(!string.IsNullOrEmpty(tokenResponse.Token));
    }

    [Fact]
    public async Task UserInfo_AfterLogin_ReturnsUserInfo()
    {
        var client = Factory.CreateClient();
        
        await RegisterValidTestUser(client);
        var tokenResponse = await LoginValidTestUser(client);
               
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenResponse.Token);
        var userInfoResponse = await client.GetAsync(Endpoint.AuthUserInfoEndpoint);
        Assert.Equal(HttpStatusCode.OK, userInfoResponse.StatusCode);
    }
    #endregion
    
    #region Helpers
    
    private async Task RegisterValidTestUser(
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

    private async Task<TokenResponse> LoginValidTestUser(
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
        var tokenReponse = await loginResponse.Content.ReadFromJsonAsync<TokenResponse>();
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        Assert.True(!string.IsNullOrEmpty(tokenReponse?.Token));

        return tokenReponse;
    }

    private async Task<StringContent> GetRegisterRequestPayload(
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
    #endregion
}