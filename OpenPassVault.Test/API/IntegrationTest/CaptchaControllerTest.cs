using System.Net.Http.Json;
using OpenPassVault.Shared.DTO;
using OpenPassVault.Test.API.Setup;

namespace OpenPassVault.Test.API.IntegrationTest;

public class CaptchaControllerTest : ControllerTestBase
{
    [Fact]
    public async Task GetCaptcha_ReturnsOk()
    {
        var client = Factory.CreateClient();
        var response = await client.GetAsync(Endpoint.CaptchaNewEndpoint);

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task GetCaptcha_DifferentCalls_ReturnsDifferentCaptchas()
    {
        var client = Factory.CreateClient();
        
        var firstResponse = await client.GetAsync(Endpoint.CaptchaNewEndpoint);
        var firstCaptchaResponse = await firstResponse.Content.ReadFromJsonAsync<NewCaptchaResponse>();
        
        var secondResponse = await client.GetAsync(Endpoint.CaptchaNewEndpoint);
        var secondCaptchaResponse = await secondResponse.Content.ReadFromJsonAsync<NewCaptchaResponse>();

        Assert.NotNull(firstCaptchaResponse);
        Assert.NotNull(secondCaptchaResponse);
        Assert.NotEqual(firstCaptchaResponse.CaptchaHmac, secondCaptchaResponse.CaptchaHmac);
        Assert.NotEqual(firstCaptchaResponse.CaptchaImageBase64, secondCaptchaResponse.CaptchaImageBase64);
    }
}