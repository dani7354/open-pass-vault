using System.Net;
using OpenPassVault.Test.API.Setup;

namespace OpenPassVault.Test.API.IntegrationTest;

public class SecretControllerTest : ControllerTestBase
{
    public static IEnumerable<object[]> EndpointsWithAuthentication =>
        new List<object[]>
        {
            new object[] { Endpoint.SecretBaseEndpoint, "GET" },
            new object[] { Endpoint.SecretBaseEndpoint, "POST" },
            new object[] { $"{Endpoint.SecretBaseEndpoint}/{Guid.NewGuid().ToString()}", "GET" },
            new object[] { $"{Endpoint.SecretBaseEndpoint}/{Guid.NewGuid().ToString()}", "DELETE" },
            new object[] { $"{Endpoint.SecretBaseEndpoint}/{Guid.NewGuid().ToString()}", "PUT" },
            new object[] { Endpoint.SecretBatchUpdateEndpoint, "PUT" },
        };
    
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
            "POST" => await client.PostAsync(endpoint, null!),
            "PUT" => await client.PutAsync(endpoint, null!),
            "DELETE" => await client.DeleteAsync(endpoint),
            _ => throw new NotSupportedException($"HTTP method {httpMethod} is not supported in this test.")
        };

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}