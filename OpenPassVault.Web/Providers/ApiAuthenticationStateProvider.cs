using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;
using OpenPassVault.Shared.Auth;
using OpenPassVault.Shared.DTO;
using OpenPassVault.Web.Models;
using OpenPassVault.Web.Services.Interfaces;

namespace OpenPassVault.Web.Providers;

public class ApiAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ClaimsPrincipal _defaultPrincipal = new(new ClaimsIdentity());
    private readonly IAuthService _authService;
    public User? CurrentUser { get; private set; }

    public ApiAuthenticationStateProvider(IAuthService authService)
    {
        _authService = authService;
        AuthenticationStateChanged += OnAuthenticationStateChangedAsync;
    }
    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var principal = await _authService.GetClaimsPrincipalFromToken() ?? _defaultPrincipal;
        return new AuthenticationState(principal);
    }

    public async Task AuthenticateUser(LoginRequest loginRequest)
    {
        var principal = await _authService.LoginAsync(loginRequest) ?? _defaultPrincipal;
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }

    public Task Logout()
    {
        CurrentUser = null;
        _authService.LogoutAsync();
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        return Task.CompletedTask;
    }
    
    private async void OnAuthenticationStateChangedAsync(Task<AuthenticationState> task)
    {
        var authenticationState = await task;
        if (authenticationState.User.Identity is not { IsAuthenticated: true })
            CurrentUser = null;
        else
            CurrentUser = GetUserFromClaimsPrincipal(authenticationState.User);
    }
    
    private User? GetUserFromClaimsPrincipal(ClaimsPrincipal principal)
    {
        var name = principal.Claims.First(c => c.Type == JwtRegisteredClaimNames.Name).Value;
        var email = principal.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value;
        var masterPasswordHash = principal.Claims.First(
            c => c.Type == JwtClaimType.TokenMasterPasswordHashClaimType).Value;

        return new User(Name: name, Email: email, MasterPasswordHash: masterPasswordHash);
    }
}