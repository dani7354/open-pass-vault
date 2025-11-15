using System.Net;
using OpenPassVault.Shared.DTO;
using OpenPassVault.Test.API.Setup;
using System.Text;
using OpenPassVault.API.Helpers;
using OpenPassVault.Shared.Crypto;

namespace OpenPassVault.Test.API.IntegrationTest;

public class AuthControllerTest : ControllerTestBase
{
    public static IEnumerable<object[]> EndpointsWithAuthentication =>
        new List<object[]>
        {
            new object[] { Endpoint.AuthUserInfoEndpoint, "GET" },
            new object[] { Endpoint.AuthDeleteEndpoint, "DELETE" },
        };
    
    [Theory]
    [MemberData(nameof(EndpointsWithAuthentication))]
    public async Task EndpointsRequireAuthentication_NotAuthenticated_ReturnsUnauthorized(
        string endpoint, 
        string httpMethod)
    {
        var client = Factory.CreateClient();
        HttpResponseMessage? response = httpMethod switch
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
        var registerPayload = await GetRegisterRequestPayload();

        var registerResponse = await client.PostAsync(Endpoint.AuthRegisterEndpoint, registerPayload);

        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);
    }

    [Fact]
    public async Task Login_ValidInput_ReturnsSuccess()
    {
        var client = Factory.CreateClient();
        string email = "a-mail@gmail.com", password = "StrongPassword123!";
        var registerRequestPayload = await GetRegisterRequestPayload(
            email: email, 
            password: password, 
            confirmPassword: password);

        var response = await client.PostAsync(Endpoint.AuthRegisterEndpoint, registerRequestPayload);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var loginRequest = new LoginRequest
        {
            Email = email,
            Password = password
        };
        var loginRequestPayload = HttpContentHelper.CreateStringContent(loginRequest);
        var loginResponse = await client.PostAsync(Endpoint.AuthLoginEndpoint, loginRequestPayload);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
    }

    private async Task<StringContent> GetRegisterRequestPayload(
        string email = "mail@mail.com",
        string password = "StrongPassword123!",
        string confirmPassword = "StrongPassword123!",
        string masterPasswordHash = "MasterPasswordHashExample")
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