using Microsoft.AspNetCore.Authentication.JwtBearer;
using OpenPassVault.Test.API.Setup;

namespace OpenPassVault.Test.API.IntegrationTest;

public class CsrfProtectionTest : ControllerTestBase
{
    [Fact]
    public async Task CsrfToken_IsboundToSessionAndNotReused()
    {
        var authenticatedClient = Factory.CreateClient();
        await AuthRequestHelper.RegisterValidTestUser(authenticatedClient);
        
        var (firstCsrfTokenCookie, _) = await AuthRequestHelper.LoginValidTestUser(authenticatedClient);
        var (secondCsrfTokenCookie, _) = await AuthRequestHelper.LoginValidTestUser(authenticatedClient);
        
        Assert.NotNull(firstCsrfTokenCookie);
        Assert.NotNull(secondCsrfTokenCookie);
        var firstTokenValue = ParseCsrfTokenFromCookie(firstCsrfTokenCookie);
        var secondTokenValue = ParseCsrfTokenFromCookie(secondCsrfTokenCookie);
        Assert.NotEqual(firstTokenValue, secondTokenValue);
    }
    
    [Fact]
    public async Task CsrfToken_MissingToken_ReturnsForbidden()
    {
        var authenticatedClient = await SetupAuthenticatedClientWithoutCsrfToken();
        var request = new HttpRequestMessage(HttpMethod.Post, Endpoint.SecretBaseEndpoint);
        
        var response = await authenticatedClient.SendAsync(request);
        var responseContent = (await response.Content.ReadAsStringAsync()).ToLower();
        
        Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Contains("csrf", responseContent);
        Assert.Contains("token", responseContent);
    }
    
    private async Task<HttpClient> SetupAuthenticatedClientWithoutCsrfToken()
    {
        var authenticatedClient = Factory.CreateClient();
        await AuthRequestHelper.RegisterValidTestUser(authenticatedClient);
        var (_, tokenResponse) = await AuthRequestHelper.LoginValidTestUser(authenticatedClient);
        
        authenticatedClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                JwtBearerDefaults.AuthenticationScheme, 
                tokenResponse.Token);
        
        return authenticatedClient;
    }
    
    private static string ParseCsrfTokenFromCookie(string csrfTokenCookie)
    {
        var startIndex = csrfTokenCookie.IndexOf('=') + 1;
        var length = csrfTokenCookie.IndexOf(';') - startIndex;
        
        return csrfTokenCookie.Substring(startIndex, length);
    }
}