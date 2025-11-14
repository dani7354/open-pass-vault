using Blazor.SubtleCrypto;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OpenPassVault.Shared.Crypto;
using OpenPassVault.Shared.Crypto.Interfaces;
using OpenPassVault.Web;
using OpenPassVault.Web.Extensions;
using OpenPassVault.Web.Providers;
using OpenPassVault.Web.Services;
using OpenPassVault.Web.Services.Interfaces;


const string httpClientName = "ApiClient";

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Configuration.AddJsonFile("appsettings.json", true, true);
builder.Configuration.AddJsonFile("appsettings.Development.json", true, true);
builder.Configuration.AddJsonFile("appsettings.Production.json", true, true);

builder.Services.AddSubtleCrypto();
builder.Services.AddSingleton<IMemoryStorageService, MemoryStorageService>();
builder.Services.AddScoped<IAccessTokenStorage, AccessTokenStorage>();
builder.Services.AddScoped<IMasterPasswordStorage, MasterPasswordMemoryStorage>();

var apiBaseAddress = builder.Configuration["ApiBaseUrl"] ?? 
                             throw new KeyNotFoundException("ApiBaseAddress is not configured");
builder.Services.AddTransient<CookieHttpHandler>();
builder.Services.AddScoped(provider => provider.GetService<IHttpClientFactory>()!.CreateClient(httpClientName))
    .AddHttpClient(httpClientName, client => client.BaseAddress = new Uri(apiBaseAddress))
    .AddHttpMessageHandler<CookieHttpHandler>();

builder.Services.AddScoped<IHttpApiService, HttpApiService>(provider => new HttpApiService(
    provider.GetService<IAccessTokenStorage>()!, provider.GetService<HttpClient>()!));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ApiAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(
    provider => provider.GetRequiredService<ApiAuthenticationStateProvider>());
builder.Services.AddScoped<ISymmetricKeyGenerator, KeyGenerator>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IEncryptionService, SubtleCryptoWrapper>();
builder.Services.AddScoped<ISecretService, SecretService>();
builder.Services.AddScoped<IUserUpdateService, UserUpdateService>();
builder.Services.AddScoped<ICaptchaApiService, CaptchaApiService>();

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

await builder.Build().RunAsync();
