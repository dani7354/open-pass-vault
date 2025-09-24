using Blazor.SubtleCrypto;
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

builder.Services.AddSubtleCrypto();
builder.Services.AddSingleton<IMemoryStorageService, MemoryStorageService>();
builder.Services.AddScoped<IAccessTokenStorage, AccessTokenStorage>();
builder.Services.AddScoped<IMasterPasswordStorage, MasterPasswordMemoryStorage>();

builder.Services.AddScoped<IHttpApiService, HttpApiService>(
    provider => new HttpApiService(provider.GetService<IAccessTokenStorage>()!, "https://localhost:8080/api/"));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ApiAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(
    provider => provider.GetRequiredService<ApiAuthenticationStateProvider>());
builder.Services.AddScoped<ISymmetricKeyGenerator, KeyGenerator>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IEncryptionService, SubtleCryptoWrapper>();
builder.Services.AddScoped<ISecretService, SecretService>();

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

await builder.Build().RunAsync();
