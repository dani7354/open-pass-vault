using OpenPassVault.Web.Models;

namespace OpenPassVault.Web.Services.Interfaces;

public interface IUserUpdateService
{
    Task UpdateUserInfo(EditUserViewModel editUserViewModel);
}