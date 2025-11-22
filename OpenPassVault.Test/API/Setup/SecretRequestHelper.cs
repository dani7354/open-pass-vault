using System.Net;
using System.Net.Http.Json;
using OpenPassVault.Shared.DTO;

namespace OpenPassVault.Test.API.Setup;

public static class SecretRequestHelper
{
    public const string TestSecretUsername = "testuser@domain.com";
    public const string TestSecretType = "Account";
    public const string TestSecretDescription = "My description...";
    public const string TestSecretName = "My Account";
    public const string TestSecretContent = "U3VwZXJTZWNyZXRDb250ZW50Cg==";  // SuperSecretContent

    public static async Task<SecretDetailsResponse> CreateTestSecretAsync(HttpClient client, StringContent content)
    {
        var response = await client.PostAsync(Endpoint.SecretBaseEndpoint, content);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdSecret = await response.Content.ReadFromJsonAsync<SecretDetailsResponse>();
        Assert.NotNull(createdSecret);
        
        return createdSecret;
    }
    
    public static StringContent CreateSecretRequestPayload(
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
}