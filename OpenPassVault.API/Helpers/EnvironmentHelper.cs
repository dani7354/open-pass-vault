using System.Reflection.Metadata;
using OpenPassVault.API.Shared;

namespace OpenPassVault.API.Helpers;

public class EnvironmentHelper
{
    private const string MysqlServer = "MYSQL_SERVER";
    private const string MysqlDatabase = "MYSQL_DATABASE";
    private const string MysqlUser = "MYSQL_USER";
    private const string MysqlPassword = "MYSQL_PASSWORD";
    
    private const string JwtSigningKey = "JWT_SIGNING_KEY";
    
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
        var mysqlServer = Environment.GetEnvironmentVariable(MysqlServer) ??
                          throw new KeyNotFoundException($"{MysqlServer} not set!");
        var mysqlDatabase = Environment.GetEnvironmentVariable(MysqlDatabase) ??
                            throw new KeyNotFoundException($"{MysqlDatabase} not set!");
        var mysqlUser = Environment.GetEnvironmentVariable(MysqlUser) ??
                        throw new KeyNotFoundException($"{MysqlUser} not set!");
        var mysqlPassword = Environment.GetEnvironmentVariable(MysqlPassword) ??
                            throw new KeyNotFoundException($"{MysqlPassword} not set!");

        return $"server={mysqlServer}; database={mysqlDatabase}; user={mysqlUser}; password={mysqlPassword}";
    }

    public static byte[] GetJwtSigningKey()
    {
        var signingKey = Environment.GetEnvironmentVariable(JwtSigningKey);
        if (string.IsNullOrEmpty(signingKey))
            throw new KeyNotFoundException($"{JwtSigningKey} not set!");
        
        return Convert.FromHexString(signingKey);
    }
}