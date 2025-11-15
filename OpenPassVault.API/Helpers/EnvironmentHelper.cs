using OpenPassVault.API.Shared;

namespace OpenPassVault.API.Helpers;

public static class EnvironmentHelper
{
    private const char MultiValueDelimiter = ';';
    
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
        var mysqlServer = GetEnvVariableOrFail(EnvironmentVariable.MysqlServer);
        var mysqlDatabase = GetEnvVariableOrFail(EnvironmentVariable.MysqlDatabase);
        var mysqlUser = GetEnvVariableOrFail(EnvironmentVariable.MysqlUser);
        var mysqlPassword = GetEnvVariableOrFail(EnvironmentVariable.MysqlPassword);

        return $"server={mysqlServer}; database={mysqlDatabase}; user={mysqlUser}; password={mysqlPassword}";
    }

    public static byte[] GetJwtSigningKey()
    {
        var signingKey = GetEnvVariableOrFail(EnvironmentVariable.JwtSigningKey);
        return Convert.FromHexString(signingKey);
    }
    
    public static string GetJwtAudience() => GetEnvVariableOrFail(EnvironmentVariable.JwtAudience);
    
    
    public static string GetJwtIssuer() => GetEnvVariableOrFail(EnvironmentVariable.JwtIssuer);

    public static byte[] GetCsrfTokenKey()
    {
        var key = GetEnvVariableOrFail(EnvironmentVariable.CsrfTokenKey);
        return Convert.FromHexString(key);
    }
    
    public static string[] GetCorsAllowedOrigins()
    {
        var origins = GetEnvVariableOrFail(EnvironmentVariable.CorsAllowedOrigins);
        return origins.Split(MultiValueDelimiter, StringSplitOptions.RemoveEmptyEntries);
    }
    
    private static string GetEnvVariableOrFail(string key)
    {
        return Environment.GetEnvironmentVariable(key) ?? 
               throw new KeyNotFoundException($"{key} missing from environment!");
    }
}