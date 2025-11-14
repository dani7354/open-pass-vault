using OpenPassVault.Web.Models;
using OpenPassVault.Web.Providers;
using OpenPassVault.Web.Services.Exceptions;
using OpenPassVault.Web.Services.Interfaces;

namespace OpenPassVault.Web.Services;

public class UserUpdateService(
    ApiAuthenticationStateProvider authenticationStateProvider,
    IAuthService authService, 
    ISecretService secretService, 
    IMasterPasswordStorage masterPasswordStorage,
    ILogger<UserUpdateService> logger) : IUserUpdateService
{
    public async Task UpdateUserInfo(EditUserViewModel editUserViewModel)
    {
        var currentMasterPassword = await GetCurrentMasterPasswordOrFail();
        var existingUser = await authService.GetUserInfo();
        await authService.UpdateUserInfo(editUserViewModel);
        
        if (!string.IsNullOrEmpty(editUserViewModel.NewMasterPassword))
        {
            var newMasterPassword = editUserViewModel.NewMasterPassword;
            await secretService.ReencryptAllSecrets(currentMasterPassword, newMasterPassword);
            currentMasterPassword = newMasterPassword;
        }
        
        await LoginAfterUpdate(editUserViewModel, existingUser.Email, currentMasterPassword);
    }

    private async Task LoginAfterUpdate(
        EditUserViewModel editUserViewModel, 
        string email, 
        string masterPassword)
    {
        var password = editUserViewModel.NewPassword ?? editUserViewModel.CurrentPassword;
        var loginViewModel = new LoginViewModel
        {
            Email = email,
            Password = password,
            MasterPassword = masterPassword
        };
        
        await authenticationStateProvider.AuthenticateUser(loginViewModel);
    }

    private async Task<string> GetCurrentMasterPasswordOrFail()
    {
        var masterPassword = await masterPasswordStorage.GetMasterPassword();
        if (string.IsNullOrEmpty(masterPassword))
        {
            logger.LogError("Master-password is not set. Someting went wrong. Logging out...");
            await authenticationStateProvider.Logout();
            throw new AuthenticationException("Master-password is not set.");
        }

        return masterPassword;
    }
}