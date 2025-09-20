using OpenPassVault.Shared.DTO;
using OpenPassVault.Web.Models;

namespace OpenPassVault.Web.Services.Interfaces;

public interface ISecretService
{
    Task<IList<SecretListItemResponse>> ListSecrets();
    Task CreateSecret(SecretCreateViewModel secretCreateViewModel);
}