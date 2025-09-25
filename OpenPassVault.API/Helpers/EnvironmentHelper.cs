using OpenPassVault.API.Shared;

namespace OpenPassVault.API.Helpers;

public static class EnvironmentHelper
{
    private const char MultiValueDelimiter = ';';
    
    private const string MysqlServer = "MYSQL_SERVER";
    private const string MysqlDatabase = "MYSQL_DATABASE";
    private const string MysqlUser = "MYSQL_USER";
    private const string MysqlPassword = "MYSQL_PASSWORD";
    
    private const string JwtSigningKey = "JWT_SIGNING_KEY";
    private const string JwtIssuer = "JWT_ISSUER";
    private const string JwtAudience = "JWT_AUDIENCE";
    
    private const string CorsAllowedOrigins = "CORS_ALLOWED_ORIGINS";
    
    public static void LoadVariablesFromEnvFile()
    {
        if (Environment.GetEnvironmentVariable(Constants.AspNetCodeEnvironment) == Constants.ProductionEnvironment)
            return;

        var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
        if (File.Exists(envPath))
        {
            File.ReadAllLines(envPath)
                .Select(line => line.Split("="))
                .Where(parts => parts.Length == 2)
                .ToList()
                .ForEach(parts => Environment.SetEnvironmentVariable(parts[0], parts[1]));
        }
    }
    
    public static string GetConnectionString()
    {
        var mysqlServer = GetEnvVariableOrFail(MysqlServer);
        var mysqlDatabase = GetEnvVariableOrFail(MysqlDatabase);
        var mysqlUser = GetEnvVariableOrFail(MysqlUser);
        var mysqlPassword = GetEnvVariableOrFail(MysqlPassword);

        return $"server={mysqlServer}; database={mysqlDatabase}; user={mysqlUser}; password={mysqlPassword}";
    }

    public static byte[] GetJwtSigningKey()
    {
        var signingKey = GetEnvVariableOrFail(JwtSigningKey);
        return Convert.FromHexString(signingKey);
    }
    
    public static string GetJwtAudience() => GetEnvVariableOrFail(JwtAudience);
    
    
    public static string GetJwtIssuer() => GetEnvVariableOrFail(JwtIssuer);
    
    public static string[] GetCorsAllowedOrigins()
    {
        var origins = GetEnvVariableOrFail(CorsAllowedOrigins);
        return origins.Split(MultiValueDelimiter, StringSplitOptions.RemoveEmptyEntries);
    }
    
    private static string GetEnvVariableOrFail(string key)
    {
        return Environment.GetEnvironmentVariable(key) ?? 
               throw new KeyNotFoundException($"{key} missing from environment!");
    }
}