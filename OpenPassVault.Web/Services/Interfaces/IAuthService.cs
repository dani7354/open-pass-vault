using System.Security.Claims;
using OpenPassVault.Web.Models;

namespace OpenPassVault.Web.Services.Interfaces;

public interface IAuthService
{
    Task<ClaimsPrincipal?> GetClaimsPrincipalFromToken();
    Task RegisterAsync(RegisterViewModel registerViewModel);
    Task<RegisterViewModel> CreateRegisterViewModel();
    Task<RegisterViewModel> RefreshRegisterViewModel(RegisterViewModel viewModel);
    Task<ClaimsPrincipal?> LoginAsync(LoginViewModel loginDto);
    Task<EditUserViewModel> CreateEditUserViewModel();
    Task UpdateUserInfo(EditUserViewModel editUserViewModel);
    Task LogoutAsync();
}
