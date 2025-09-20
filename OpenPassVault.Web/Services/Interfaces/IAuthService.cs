using System.Security.Claims;
using OpenPassVault.Shared.DTO;
using OpenPassVault.Web.Models;

namespace OpenPassVault.Web.Services.Interfaces;

public interface IAuthService
{
    ClaimsPrincipal? GetClaimsPrincipalFromToken();
    Task RegisterAsync(RegisterViewModel registerViewModel);
    Task<ClaimsPrincipal?> LoginAsync(LoginRequest loginDto);
    void LogoutAsync();
}
