namespace OpenPassVault.API.Shared;

public static class Constants
{
    public const string AspNetCodeEnvironment = "ASPNETCORE_ENVIRONMENT";

    public const string DevelopmentEnvironment = "Development";
    public const string ProductionEnvironment = "Production";

    public static class Cookies
    {
        public const string CsrfToken = "csrf_token";
    }

    public static class Headers
    {
        public const string CsrfToken = "X-CSRF-Token";
        public const string Authorization = "Authorization";
    }
}