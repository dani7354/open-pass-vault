using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using OpenPassVault.API.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenPassVault.API.Data.DataContext;
using OpenPassVault.API.Data.Interfaces;
using OpenPassVault.API.Data.Repository;
using OpenPassVault.API.Helpers;
using OpenPassVault.API.Middleware;
using OpenPassVault.API.Services;
using OpenPassVault.API.Services.Interfaces;
using OpenPassVault.Shared.Crypto;
using OpenPassVault.Shared.Crypto.Interfaces;
using OpenPassVault.Shared.Services;
using OpenPassVault.Shared.Services.Interfaces;

namespace OpenPassVault.API;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services, IWebHostBuilder webHostBuilder)
    {
        webHostBuilder.ConfigureKestrel(options =>
        {
            options.AddServerHeader = false;
            options.ConfigureHttpsDefaults(httpsOptions =>
            {
                httpsOptions.SslProtocols = System.Security.Authentication.SslProtocols.Tls12 |
                                            System.Security.Authentication.SslProtocols.Tls13;
            });
        });

        services.AddOpenApi();
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddHttpContextAccessor();
       
        EnvironmentHelper.LoadVariablesFromEnvFile();
        var dbConnectionString = EnvironmentHelper.GetConnectionString();
        services.AddDbContext<ApplicationDatabaseContext>(
            options => options.UseMySql(dbConnectionString, ServerVersion.AutoDetect(dbConnectionString)));

        services.AddScoped<ISecretRepository, SecretRepository>();
        services.AddScoped<ISecretService, SecretService>();
        
        ConfigureWebSecurityServices(services);
        ConfigureAuthentication(services);
    }

    private void ConfigureWebSecurityServices(IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(p =>
                p.WithOrigins(EnvironmentHelper.GetCorsAllowedOrigins())
                    .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                    .AllowAnyHeader()
                    .AllowCredentials());
        });
        
        var csrfTokenKey = EnvironmentHelper.GetCsrfTokenKey();
        services.AddScoped<IHmacService, HmacService>(_ => new HmacService(csrfTokenKey));
        services.AddScoped<ICaptchaService, CaptchaService>();
        services.AddScoped<ICsrfTokenService, CsrfTokenService>();
        services.AddScoped<CsrfProtection>();
        services.AddSingleton<SecurityHeaders>();
    }

    private void ConfigureAuthentication(IServiceCollection services)
    {
        var signingToken = EnvironmentHelper.GetJwtSigningKey();
        var tokenIssuer = EnvironmentHelper.GetJwtIssuer();
        var tokenAudience = EnvironmentHelper.GetJwtAudience();
        services.AddScoped<IAccessTokenService, AccessTokenService>(
            _ => new AccessTokenService(signingToken, tokenAudience, tokenIssuer));

        services.AddIdentityCore<ApiUser>(o =>
        {
            o.Password.RequireDigit = false;
            o.Password.RequireLowercase = false;
            o.Password.RequireUppercase = false;
            o.Password.RequireNonAlphanumeric = false;
            o.Password.RequiredLength = 16;
        }).AddEntityFrameworkStores<ApplicationDatabaseContext>().AddSignInManager();

        services.AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            o.RequireHttpsMetadata = false;
            o.SaveToken = true;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidAudience = tokenAudience,
                ValidIssuer = tokenIssuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(signingToken),
                ValidateIssuer = true,
                ValidateAudience = false,
                RequireExpirationTime = true,
                ValidateLifetime = true
            };
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsProduction())
        {
            app.UseSecurityHeaders();
        }

        app.UseCsrfProtection();
        app.UseCors();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            if (env.IsDevelopment())
            {
                endpoints.MapOpenApi();
            }
        });
    }
}
