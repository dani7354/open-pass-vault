namespace OpenPassVault.Test.API;

public static class Endpoint
{
    public const string AuthBaseEndpoint = "/api/auth";
    public const string AuthUserInfoEndpoint = $"{AuthBaseEndpoint}/user-info";
    public const string AuthDeleteEndpoint = $"{AuthBaseEndpoint}/delete";
}