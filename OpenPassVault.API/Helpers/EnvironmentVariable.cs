namespace OpenPassVault.API.Helpers;

public static class EnvironmentVariable
{
    public const string MysqlServer = "MYSQL_SERVER";
    public const string MysqlDatabase = "MYSQL_DATABASE";
    public const string MysqlUser = "MYSQL_USER";
    public const string MysqlPassword = "MYSQL_PASSWORD";
    
    public const string JwtSigningKey = "JWT_SIGNING_KEY";
    public const string JwtIssuer = "JWT_ISSUER";
    public const string JwtAudience = "JWT_AUDIENCE";
    
    public const string CsrfTokenKey = "CSRF_TOKEN_KEY";
    
    public const string CorsAllowedOrigins = "CORS_ALLOWED_ORIGINS";
}