using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using OpenPassVault.API.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenPassVault.API.Data.DataContext;
using OpenPassVault.API.Data.Interfaces;
using OpenPassVault.API.Data.Repository;
using OpenPassVault.API.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

EnvironmentHelper.LoadVariablesFromEnvFile();
var dbConnectionString = EnvironmentHelper.GetConnectionString();
builder.Services.AddDbContext<ApplicationDatabaseContext>(
    options => options.UseMySql(dbConnectionString, ServerVersion.AutoDetect(dbConnectionString)));

builder.Services.AddScoped<ISecretRepository, SecretRepository>();

//builder.Services.AddIdentityApiEndpoints<ApiUser>()
//    .AddEntityFrameworkStores<ApplicationDatabaseContext>();

builder.Services.AddIdentity<ApiUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDatabaseContext>();
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
        IssuerSigningKey = new SymmetricSecurityKey(EnvironmentHelper.GetJwtSigningKey()),
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

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
