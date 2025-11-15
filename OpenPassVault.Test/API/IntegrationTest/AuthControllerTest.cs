using System.Net;
using OpenPassVault.Test.API.Setup;

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
}