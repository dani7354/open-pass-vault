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
        var captchaCode = "xy45xxswr";
        var key = EnvironmentHelper.GetCsrfTokenKey();
        var hmacService = new HmacService(key);
        var captchaHmac = await hmacService.CreateHmac(Encoding.UTF8.GetBytes(captchaCode));
        
        var password = "StrongPassword123!";
        var masterPasswordHash = "MasterPasswordHashExample";
        var userRegisterRequest = new RegisterRequest
        {
            Email = "mail@mail.com",
            Password = password,
            ConfirmPassword = password,
            MasterPasswordHash = masterPasswordHash,
            CaptchaCode = captchaCode,
            CaptchaHmac = captchaHmac
        };
        var registerPayload = HttpContentHelper.CreateStringContent(userRegisterRequest);

        var registerResponse = await client.PostAsync(Endpoint.AuthRegisterEndpoint, registerPayload);

        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);
    }
}