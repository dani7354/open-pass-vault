using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using OpenPassVault.Web.Services.Interfaces;

namespace OpenPassVault.Web.Providers;

public class JwtAuthenticationProvider(IAuthService authService) : AuthenticationStateProvider
{
    private readonly IAuthService _authService = authService;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // var tokenHandler = new JwtSecurityTokenHandler();
        //var identity = new ClaimsIdentity();

        /*
        if (tokenHandler.CanReadToken(token))
        {
            var jwtSecurityToken = tokenHandler.ReadJwtToken(token);
            identity = new(jwtSecurityToken.Claims, "Blazor School");
        }

        return new(identity);*/
    }
}