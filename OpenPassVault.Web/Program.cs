using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OpenPassVault.Web;
using OpenPassVault.Web.Providers;
using OpenPassVault.Web.Services;
using OpenPassVault.Web.Services.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<IHttpApiService, HttpApiService>(x => new HttpApiService("http://localhost:5000")); //TODO: change URL!
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBrowserStorageService, BrowserStorageService>();
builder.Services.AddScoped<JwtAuthenticationProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(x =>  x.GetRequiredService<JwtAuthenticationProvider>());
builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();