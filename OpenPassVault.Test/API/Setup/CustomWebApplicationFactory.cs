using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenPassVault.API.Data.DataContext;
using OpenPassVault.API.Data.Entity;
using OpenPassVault.API.Helpers;

namespace OpenPassVault.Test.API.Setup;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Overwrite the following environment variables if necessary! 
        Environment.SetEnvironmentVariable(EnvironmentVariable.MysqlServer, "localhost");
        Environment.SetEnvironmentVariable(EnvironmentVariable.MysqlDatabase, "db");
        Environment.SetEnvironmentVariable(EnvironmentVariable.MysqlUser, "user");
        Environment.SetEnvironmentVariable(EnvironmentVariable.MysqlPassword, new string('0', 16));
        Environment.SetEnvironmentVariable(EnvironmentVariable.JwtAudience, "localhost");
        Environment.SetEnvironmentVariable(EnvironmentVariable.JwtIssuer, "localhost");
        Environment.SetEnvironmentVariable(EnvironmentVariable.JwtSigningKey, new string('0', 32));
        Environment.SetEnvironmentVariable(EnvironmentVariable.CsrfTokenKey, new string('0', 64));
        Environment.SetEnvironmentVariable(EnvironmentVariable.CorsAllowedOrigins, "http://localhost");
        
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(ApplicationDatabaseContext));
            services.RemoveAll(typeof(DbContextOptions<ApplicationDatabaseContext>));
            services.RemoveAll(typeof(IDbContextOptionsConfiguration<ApplicationDatabaseContext>));
            
            services.AddDbContext<ApplicationDatabaseContext>(
                options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);

            services.AddIdentityCore<ApiUser>()
                .AddEntityFrameworkStores<ApplicationDatabaseContext>();
        });

        builder.UseEnvironment("Development");
    }
}