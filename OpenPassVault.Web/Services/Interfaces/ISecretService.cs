using OpenPassVault.Shared.DTO;
using OpenPassVault.Web.Models;

namespace OpenPassVault.Web.Services.Interfaces;

public interface ISecretService
{
    Task<IList<SecretListItemResponse>> ListSecrets();
    Task<SecretDetailsResponse?> GetSecret(string id);
    Task CreateSecret(SecretCreateViewModel secretCreateViewModel);
    Task DeleteSecret(string id);
    Task<string> DecryptSecretContent(string content);
    Task ReencryptAllSecrets(string oldMasterPassword, string newMasterPassword);
    Task<IList<SecretTypeViewModel>> GetSecretTypes();
}