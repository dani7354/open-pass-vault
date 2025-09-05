using Microsoft.AspNetCore.Components.Authorization;
using OpenPassVault.Web.Services.Interfaces;

namespace OpenPassVault.Web.Providers;

public class JwtAuthenticationProvider(IAuthService authService) : AuthenticationStateProvider
{
    private readonly IAuthService _authService = authService;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        throw new NotImplementedException();
    }
}