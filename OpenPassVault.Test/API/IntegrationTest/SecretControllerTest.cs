using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Net.Http.Headers;
using OpenPassVault.Shared.DTO;
using OpenPassVault.Test.API.Setup;

namespace OpenPassVault.Test.API.IntegrationTest;

public class SecretControllerTest : ControllerTestBase
{
    private const string TestSecretUsername = "testuser@domain.com";
    private const string TestSecretType = "Account";
    private const string TestSecretDescription = "My description...";
    private const string TestSecretName = "My Account";
    private const string TestSecretContent = "SuperSecretContent";
    
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

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var secrets = await response.Content.ReadFromJsonAsync<SecretListItemResponse[]>();
        Assert.NotNull(secrets);
        Assert.Empty(secrets);
    }
    
    [Fact]
    public async Task Secrets_AfterCreatingSecret_ReturnsCreatedSecretInList()
    {
        var createdSecret = await CreateTestSecretAsync();

        var response = await _authenticatedClient.GetAsync(Endpoint.SecretBaseEndpoint);

        response.EnsureSuccessStatusCode();

        var secrets = await response.Content.ReadFromJsonAsync<SecretListItemResponse[]>();
        Assert.NotNull(secrets);
        Assert.Single(secrets);

        var secretInList = secrets.First();
        Assert.Equal(createdSecret.Id, secretInList.Id);
        Assert.Equal(createdSecret.Name, secretInList.Name);
        Assert.Equal(createdSecret.Type, secretInList.Type);
    }

    [Fact]
    public async Task CreateSecret_ValidInput_ReturnsCreatedSecret()
    {
        var createdSecret = await CreateTestSecretAsync();
        
        Assert.NotNull(createdSecret);
        Assert.Equal(TestSecretName, createdSecret.Name);
        Assert.Equal(TestSecretType, createdSecret.Type);
        Assert.Equal(TestSecretUsername, createdSecret.Username);
        Assert.Equal(TestSecretContent, createdSecret.Content);
        Assert.Equal(TestSecretDescription, createdSecret.Description);
    }
    
    [Fact]
    public async Task UpdateSecret_ValidInput_ReturnsUpdatedSecret()
    {
        var createdSecret = await CreateTestSecretAsync();

        var updatedName = "Updated Secret Name";
        var updatedContent = "UpdatedSecretContent";
        var updateRequest = new SecretUpdateRequest
        {
            Id = createdSecret.Id,
            Description = createdSecret.Description,
            Type = createdSecret.Type,
            Username = createdSecret.Username,
            Name = updatedName,
            Content = updatedContent
        };
        var updateRequestPayload = HttpContentHelper.CreateStringContent(updateRequest);

        var response = await _authenticatedClient.PutAsync(
            $"{Endpoint.SecretBaseEndpoint}/{createdSecret.Id}", 
            updateRequestPayload);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task DeleteSecret_ExistingSecret_ReturnsNoContent()
    {
        var createdSecret = await CreateTestSecretAsync();

        var response = await _authenticatedClient.DeleteAsync(
            $"{Endpoint.SecretBaseEndpoint}/{createdSecret.Id}");
        
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    
    private async Task<SecretDetailsResponse> CreateTestSecretAsync()
    {
        var requestPayload = CreateSecretRequestPayload();
        var response = await _authenticatedClient.PostAsync(Endpoint.SecretBaseEndpoint, requestPayload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdSecret = await response.Content.ReadFromJsonAsync<SecretDetailsResponse>();
        Assert.NotNull(createdSecret);
        return createdSecret;
    }

    private StringContent CreateSecretRequestPayload(
        string name = TestSecretName,
        string type = TestSecretType,
        string? username = TestSecretUsername,
        string content = TestSecretContent,
        string? description = TestSecretDescription)
    {
        var newSecretRequest = new SecretCreateRequest
        {
            Name = name,
            Type = type,
            Username = username,
            Content = content,
            Description = description
        };

        return HttpContentHelper.CreateStringContent(newSecretRequest);
    }
    
    private async Task SetupAuthenticatedClientAsync()
    {
        _authenticatedClient = Factory.CreateClient();
        await AuthRequestHelper.RegisterValidTestUser(_authenticatedClient);
        var (csrfTokenCookie, tokenResponse) = await AuthRequestHelper.LoginValidTestUser(_authenticatedClient);
        
        _authenticatedClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme, 
                tokenResponse.Token);
        
        _authenticatedClient.DefaultRequestHeaders.Add(HeaderNames.Cookie, csrfTokenCookie);
    }
}