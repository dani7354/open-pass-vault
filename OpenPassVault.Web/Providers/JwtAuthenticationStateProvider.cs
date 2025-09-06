using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using OpenPassVault.Shared.DTO;
using OpenPassVault.Web.Models;
using OpenPassVault.Web.Services.Interfaces;

namespace OpenPassVault.Web.Providers;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IAuthService _authService;
    public User? CurrentUser { get; private set; }

    public JwtAuthenticationStateProvider(IAuthService authService)
    {
        _authService = authService;
        AuthenticationStateChanged += OnAuthenticationStateChangedAsync;
    }
    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync() // TODO: implement
    {
        var principal = await _authService.GetClaimsPrincipalFromToken() ?? new ClaimsPrincipal();
        return new AuthenticationState(principal);
    }

    public async Task AuthenticateUser(LoginRequest loginRequest)
    {
        var principal = new ClaimsPrincipal();
        
        //var success = await _authService.LoginAsync(loginRequest);
        var success = true;
        if (success)
        {
            principal = await _authService.GetClaimsPrincipalFromToken() ?? new ClaimsPrincipal();
        }
        
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }

    public async Task Logout()
    {
        CurrentUser = null;
        await _authService.LogoutAsync();
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
    
    private async void OnAuthenticationStateChangedAsync(Task<AuthenticationState> task)
    {
        var authenticationState = await task;

        if (authenticationState != null)
        {
            CurrentUser = new User(Name: "Daniel", Email: "d@stuhrs.dk");
        }
    }
}