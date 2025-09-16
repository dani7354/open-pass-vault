using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OpenPassVault.Shared.Crypto;
using OpenPassVault.Shared.Crypto.Interfaces;
using OpenPassVault.Web;
using OpenPassVault.Web.Providers;
using OpenPassVault.Web.Services;
using OpenPassVault.Web.Services.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<IHttpApiService, HttpApiService>(x => new HttpApiService("http://localhost:7139"));
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMemoryStorageService, MemoryStorageService>();
builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(x =>  x.GetRequiredService<JwtAuthenticationStateProvider>());

builder.Services.AddScoped<ISymmetricKeyGenerator, KeyGenerator>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IEncryptionService, EncryptionService>();

builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();