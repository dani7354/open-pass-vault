using System.Net;
using Microsoft.Net.Http.Headers;
using OpenPassVault.Test.API.Setup;

namespace OpenPassVault.Test.API.IntegrationTest;

public class SecurityHeadersTest : ControllerTestBase
{
    private readonly string _testEndpoint;
    private readonly HttpClient _client;

    public SecurityHeadersTest()
    {
        _client = Factory.CreateClient();
        _testEndpoint = Endpoint.CaptchaNewEndpoint;
    }
    
    [Fact]
    public async Task HstsHeader_IsPresent()
    {
        var response = await _client.GetAsync(_testEndpoint);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.Contains(HeaderNames.StrictTransportSecurity));
        var headerValue = response.Headers.GetValues(HeaderNames.StrictTransportSecurity).FirstOrDefault();
        Assert.Equal("max-age=31536000; includeSubDomains; preload", headerValue);
    }
    
    [Fact]
    public async Task ContentTypeOptionsHeader_IsPresent()
    {
        var response = await _client.GetAsync(_testEndpoint);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.Contains(HeaderNames.XContentTypeOptions));
        var headerValue = response.Headers.GetValues(HeaderNames.XContentTypeOptions).FirstOrDefault();
        Assert.Equal("nosniff", headerValue);
    }
    
    [Fact]
    public async Task XFrameOptions_IsPresentAndHaveCorrentValue()
    {
        var response = await _client.GetAsync(_testEndpoint);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.Contains(HeaderNames.XFrameOptions));
        var headerValue = response.Headers.GetValues(HeaderNames.XFrameOptions).FirstOrDefault();
        Assert.Equal("DENY", headerValue);
    }
    
    [Fact]
    public async Task CorsHeaders_ArePresentAndHaveCorrectValue()
    {
        var origin = "http://localhost";
        _client.DefaultRequestHeaders.Add(HeaderNames.Origin, origin);
        var response = await _client.GetAsync(_testEndpoint);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.Contains(HeaderNames.AccessControlAllowOrigin));
        Assert.True(response.Headers.Contains(HeaderNames.AccessControlAllowCredentials));
        
        var allowOriginHeaderValue = response.Headers.GetValues(
            HeaderNames.AccessControlAllowOrigin).FirstOrDefault();
        Assert.Equal(origin, allowOriginHeaderValue);
        
        var allowCredentialsHeaderValue = response.Headers.GetValues(
            HeaderNames.AccessControlAllowCredentials).FirstOrDefault();
        Assert.Equal("true", allowCredentialsHeaderValue);
    }
}