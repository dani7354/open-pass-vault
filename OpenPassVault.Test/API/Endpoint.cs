namespace OpenPassVault.Test.API;

public static class Endpoint
{
    public const string AuthBaseEndpoint = "/api/auth";
    public const string AuthRegisterEndpoint = $"{AuthBaseEndpoint}/register";
    public const string AuthLoginEndpoint = $"{AuthBaseEndpoint}/login";
    public const string AuthUserInfoEndpoint = $"{AuthBaseEndpoint}/user-info";
    public const string AuthDeleteEndpoint = $"{AuthBaseEndpoint}/delete";
    
    public const string SecretBaseEndpoint = "/api/secrets";
    public const string SecretBatchUpdateEndpoint = $"{SecretBaseEndpoint}/batch";
}