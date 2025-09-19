using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using OpenPassVault.API.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenPassVault.API.Data.DataContext;
using OpenPassVault.API.Data.Interfaces;
using OpenPassVault.API.Data.Repository;
using OpenPassVault.API.Helpers;
using OpenPassVault.API.Services;
using OpenPassVault.API.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(p => 
        p.WithOrigins("http://localhost:5000")
            .AllowAnyMethod()
            .AllowAnyHeader());
}); 

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

EnvironmentHelper.LoadVariablesFromEnvFile();
var dbConnectionString = EnvironmentHelper.GetConnectionString();
builder.Services.AddDbContext<ApplicationDatabaseContext>(
    options => options.UseMySql(dbConnectionString, ServerVersion.AutoDetect(dbConnectionString)));

builder.Services.AddScoped<ISecretRepository, SecretRepository>();
builder.Services.AddScoped<ISecretService, SecretService>();

var signingToken = EnvironmentHelper.GetJwtSigningKey();
var tokenIssuer = EnvironmentHelper.GetJwtIssuer();
var tokenAudience = EnvironmentHelper.GetJwtAudience();
builder.Services.AddScoped<ITokenService, TokenService>(
    _ => new TokenService(signingToken, tokenAudience, tokenIssuer));

builder.Services.AddIdentity<ApiUser, IdentityRole>(o =>
{
    o.Password.RequireDigit = false;
    o.Password.RequireLowercase = false;
    o.Password.RequireUppercase = false;
    o.Password.RequireNonAlphanumeric = false;
    o.Password.RequiredLength = 16;
}).AddEntityFrameworkStores<ApplicationDatabaseContext>();

builder.Services.AddAuthentication(o =>
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
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(signingToken),
        ValidateIssuer = false,
        ValidateAudience = false,
        RequireExpirationTime = false,
        ValidateLifetime = true
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
