using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Net.Http.Headers;
using OpenPassVault.Shared.DTO;
using OpenPassVault.Test.API.Setup;

namespace OpenPassVault.Test.API.IntegrationTest;

public class SecretControllerTest : ControllerTestBase
{
    private HttpClient _authenticatedClient = null!;
    
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

    public SecretControllerTest()
    {
        SetupAuthenticatedClientAsync().GetAwaiter().GetResult();
    }
    
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
    
    [Fact]
    public async Task Secrets_NoSecrets_ReturnsEmptyArray()
    {
        var response = await _authenticatedClient.GetAsync(Endpoint.SecretBaseEndpoint);

        response.EnsureSuccessStatusCode();

        var secrets = await response.Content.ReadFromJsonAsync<SecretListItemResponse[]>();
        Assert.NotNull(secrets);
        Assert.Empty(secrets);
    }

    [Fact]
    public async Task CreateSecret_ValidInput_ReturnsCreatedSecret()
    {
        var newSecretRequest = new SecretCreateRequest
        {
            Name = "Facebook Account",
            Type = "Account",
            Username = "mail@gmail.com",
            Content = new string('*', 100),
            Description = "Min gamle Facebook-konto"
        };

        var requestPayload = HttpContentHelper.CreateStringContent(newSecretRequest);
        var response = await _authenticatedClient.PostAsync(Endpoint.SecretBaseEndpoint, requestPayload);

        response.EnsureSuccessStatusCode();

        var createdSecret = await response.Content.ReadFromJsonAsync<SecretDetailsResponse>();
        Assert.NotNull(createdSecret);
    }
    

    private async Task SetupAuthenticatedClientAsync()
    {
        _authenticatedClient = Factory.CreateClient();
        await AuthRequestHelper.RegisterValidTestUser(_authenticatedClient);
        var (csrfTokenCookie, tokenResponse) = await AuthRequestHelper.LoginValidTestUser(_authenticatedClient);
        _authenticatedClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, tokenResponse.Token);
        _authenticatedClient.DefaultRequestHeaders.Add(HeaderNames.Cookie, csrfTokenCookie);
    }
}