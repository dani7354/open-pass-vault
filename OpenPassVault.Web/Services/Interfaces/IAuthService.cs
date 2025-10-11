using System.Security.Claims;
using OpenPassVault.Shared.DTO;
using OpenPassVault.Web.Models;
using OpenPassVault.Web.Pages;

namespace OpenPassVault.Web.Services.Interfaces;

public interface IAuthService
{
    Task<ClaimsPrincipal?> GetClaimsPrincipalFromToken();
    Task RegisterAsync(RegisterViewModel registerViewModel);
    Task<RegisterViewModel> CreateRegisterViewModel();
    Task<ClaimsPrincipal?> LoginAsync(LoginViewModel loginDto);
    Task LogoutAsync();
}
