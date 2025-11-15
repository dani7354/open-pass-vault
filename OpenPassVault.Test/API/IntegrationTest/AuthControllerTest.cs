using System.Net;
using OpenPassVault.Test.API.Setup;
using OpenPassVault.API;

namespace OpenPassVault.Test.API.IntegrationTest;

public class AuthControllerTest
{
    private readonly CustomWebApplicationFactory<Startup> _factory;
    
    public AuthControllerTest()
    {
        _factory = new CustomWebApplicationFactory<Startup>();
    }

    [Fact]
    public async Task TestLogin()
    {
        var client = _factory.CreateClient();
        var res = await client.GetAsync("api/auth/user-info");
        Assert.True(res.StatusCode == (HttpStatusCode)401);
    }
    
}