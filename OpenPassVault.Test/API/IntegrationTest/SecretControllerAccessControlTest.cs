using OpenPassVault.Test.API.Setup;
using System.Net;
using OpenPassVault.Shared.DTO;

namespace OpenPassVault.Test.API.IntegrationTest;

public class SecretControllerAccessControlTest : ControllerTestBase
{
    [Fact]
    public async Task Secrets_RequireAuthentication_NotAuthenticated_ReturnsUnauthorized()
    {
        var client = Factory.CreateClient();
        var response = await client.GetAsync(Endpoint.SecretBaseEndpoint);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task Secret_RequireAuthentication_NotAuthenticated_ReturnsUnauthorized()
    {
        var client = Factory.CreateClient();
        var response = await client.GetAsync($"{Endpoint.SecretBaseEndpoint}/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Secret_TwoDifferentUsers_CannotAccessEachOthersSecrets()
    {
        string emailUserOne = "mail0@mail.dk", emailUserTwo = "mail1@mail.dk";
        var clientUserOne = await RegisterUserAndSetupAuthenticatedClient(emailUserOne);

        var createdSecretDetails = await SecretRequestHelper.CreateTestSecretAsync(
            clientUserOne, SecretRequestHelper.CreateSecretRequestPayload());
        
        var getResponseUserOne = await clientUserOne.GetAsync($"{Endpoint.SecretBaseEndpoint}/{createdSecretDetails.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponseUserOne.StatusCode);
        
        var clientUserTwo = await RegisterUserAndSetupAuthenticatedClient(emailUserTwo);
        var getResponseUserTwo = await clientUserTwo.GetAsync($"{Endpoint.SecretBaseEndpoint}/{createdSecretDetails.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponseUserTwo.StatusCode);
    }

    [Fact]
    public async Task Update_OtherUsersSecret_ReturnsNotFound()
    {
        string emailUserOne = "mail0@mail.dk", emailUserTwo = "mail1@mail.dk";
        var clientUserOne = await RegisterUserAndSetupAuthenticatedClient(emailUserOne);

        var createdSecretDetails = await SecretRequestHelper.CreateTestSecretAsync(
            clientUserOne, SecretRequestHelper.CreateSecretRequestPayload());
        
        var clientUserTwo = await RegisterUserAndSetupAuthenticatedClient(emailUserTwo);
        var updatePayload = new SecretUpdateRequest
        {
            Id = createdSecretDetails.Id,
            Name = "Updated Name",
            Username = "updated_username",
            Type = "Account",
            Content = "VXBkYXRlZENvbnRlbnQ=", // UpdatedContent
            Description = "Updated Description",
        };
        var response = await clientUserTwo.PutAsync(
            $"{Endpoint.SecretBaseEndpoint}/{createdSecretDetails.Id}",
            HttpContentHelper.CreateStringContent(updatePayload));
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    private async Task<HttpClient> RegisterUserAndSetupAuthenticatedClient(string email)
    {
        var authenticatedClient = Factory.CreateClient();
        await AuthRequestHelper.RegisterUserAndSetupAuthenticatedClient(authenticatedClient, email);
        
        return authenticatedClient;
    }
}