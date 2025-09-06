using Microsoft.AspNetCore.Identity;
using OpenPassVault.API.Data.Entity;
using Microsoft.EntityFrameworkCore;
using OpenPassVault.API.Data.DataContext;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<DatabaseContext>(options => options.UseMySql("", ServerVersion.AutoDetect("")));
builder.Services.AddIdentity<ApiUser, IdentityRole>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
