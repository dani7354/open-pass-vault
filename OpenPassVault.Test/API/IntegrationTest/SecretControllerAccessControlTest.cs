using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Net.Http.Headers;
using OpenPassVault.Test.API.Setup;
using System.Net;

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
    
    private async Task<HttpClient> RegisterUserAndSetupAuthenticatedClient(string email)
    {
        var authenticatedClient = Factory.CreateClient();
        
        await AuthRequestHelper.RegisterValidTestUser(authenticatedClient, email: email);
        var (csrfTokenCookie, accessToken) = await AuthRequestHelper.LoginValidTestUser(
            authenticatedClient, email: email);
        
        authenticatedClient.DefaultRequestHeaders.Add(
            HeaderNames.Authorization, $"{JwtBearerDefaults.AuthenticationScheme} {accessToken.Token}");
        authenticatedClient.DefaultRequestHeaders.Add(HeaderNames.Cookie, csrfTokenCookie);
        
        return authenticatedClient;
    }

}