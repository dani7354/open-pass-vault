using System.Net;
using OpenPassVault.Test.API.Setup;

namespace OpenPassVault.Test.API.IntegrationTest;

public class AuthControllerTest : ControllerTestBase
{


    public static IEnumerable<object[]> EndpointsWithAuthentication =>
        new List<object[]>
        {
            new object[] { Endpoint.AuthUserInfoEndpoint, "GET" },
            new object[] { Endpoint.AuthDeleteEndpoint, "DELETE" },
        };

    public static IEnumerable<object[]> InvalidEmailAddresses =>
        new List<object[]>
        {
            new object[] { "plainaddress" },
            new object[] { "@missingusername.com" },
            new object[] { "usern ame@.com" },
            new object[] { "username\n@com" },
            new object[] { "username@" },
        };

    #region Tests

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
            "DELETE" => await client.DeleteAsync(endpoint),
            _ => throw new NotSupportedException($"HTTP method {httpMethod} is not supported in this test.")
        };

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Register_ValidInput_ReturnsSuccess()
    {
        var client = Factory.CreateClient();
        await AuthRequestHelper.RegisterValidTestUser(client);
    }

    [Fact]
    public async Task Register_PasswordMismatch_ReturnsBadRequest()
    {
        var client = Factory.CreateClient();
        var registerRequestPayload = await AuthRequestHelper.GetRegisterRequestPayload(
            password: "Password1!",
            confirmPassword: "Password2!");

        var response = await client.PostAsync(Endpoint.AuthRegisterEndpoint, registerRequestPayload);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(InvalidEmailAddresses))]
    public async Task Register_InvalidEmail_ReturnsBadRequest(string email)
    {
        var client = Factory.CreateClient();
        var registerRequestPayload = await AuthRequestHelper.GetRegisterRequestPayload(
            email: email);

        var response = await client.PostAsync(Endpoint.AuthRegisterEndpoint, registerRequestPayload);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsBadRequest()
    {
        var client = Factory.CreateClient();
        await AuthRequestHelper.RegisterValidTestUser(client);

        var registerRequestPayload = await AuthRequestHelper.GetRegisterRequestPayload();
        var response = await client.PostAsync(Endpoint.AuthRegisterEndpoint, registerRequestPayload);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_ValidInput_ReturnsSuccess()
    {
        var client = Factory.CreateClient();
        await AuthRequestHelper.RegisterValidTestUser(client);
        var (_, tokenResponse) = await AuthRequestHelper.LoginValidTestUser(client);

        Assert.True(!string.IsNullOrEmpty(tokenResponse.Token));
    }

    [Fact]
    public async Task UserInfo_AfterLogin_ReturnsUserInfo()
    {
        var client = Factory.CreateClient();

        await AuthRequestHelper.RegisterValidTestUser(client);
        var (_, tokenResponse) = await AuthRequestHelper.LoginValidTestUser(client);

        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenResponse.Token);
        var userInfoResponse = await client.GetAsync(Endpoint.AuthUserInfoEndpoint);
        Assert.Equal(HttpStatusCode.OK, userInfoResponse.StatusCode);
    }

    #endregion
}