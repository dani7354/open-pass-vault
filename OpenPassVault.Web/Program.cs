using Blazored.SessionStorage;
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

builder.Services.AddBlazoredSessionStorageAsSingleton();

builder.Services.AddSingleton<IMemoryStorageService, MemoryStorageService>();

builder.Services.AddScoped<IHttpApiService, HttpApiService>(
    provider => new HttpApiService(provider.GetService<ISessionStorageService>()!, "http://localhost:5001/api/"));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ApiAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(
    provider => provider.GetRequiredService<ApiAuthenticationStateProvider>());
builder.Services.AddScoped<ISymmetricKeyGenerator, KeyGenerator>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IEncryptionService, EncryptionService>();

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

await builder.Build().RunAsync();