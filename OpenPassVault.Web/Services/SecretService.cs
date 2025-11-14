using OpenPassVault.Shared.Crypto.Interfaces;
using OpenPassVault.Shared.DTO;
using OpenPassVault.Web.Models;
using OpenPassVault.Web.Providers;
using OpenPassVault.Web.Services.Exceptions;
using OpenPassVault.Web.Services.Interfaces;

namespace OpenPassVault.Web.Services;

public class SecretService(
    IHttpApiService httpApiService,
    ApiAuthenticationStateProvider authenticationStateProvider,
    IEncryptionService encryptionService,
    IMasterPasswordStorage masterPasswordStorage) : ISecretService
{
    private const string BaseUrl = "secrets";
    private const string BatchUpdateUlr = $"{BaseUrl}/batch";

    public async Task<IList<SecretListItemResponse>> ListSecrets()
    {
        try
        {
            var response = await httpApiService.GetAsync<IList<SecretListItemResponse>>(BaseUrl);
            return response ?? [];
        }
        catch (ApiRequestUnauthorizedException)
        {
            await authenticationStateProvider.Logout();
            return [];
        }
    }

    public async Task<SecretDetailsResponse?> GetSecret(string id)
    {
        try
        {
            var response = await httpApiService.GetAsync<SecretDetailsResponse>($"{BaseUrl}/{id}");
            return response ?? null;
        }
        catch (ApiRequestUnauthorizedException)
        {
            await authenticationStateProvider.Logout();
            return null;
        }
    }

    public async Task CreateSecret(SecretCreateViewModel secretCreateViewModel)
    {
        var masterPassword = await masterPasswordStorage.GetMasterPassword();
        if (string.IsNullOrEmpty(masterPassword))
            throw new AuthenticationException();

        var encryptedContent =  await encryptionService.Encrypt(secretCreateViewModel.ContentPlain, masterPassword);
        var secretRequest = new SecretCreateRequest
        {
            Name = secretCreateViewModel.Name,
            Description = secretCreateViewModel.Description,
            Username = secretCreateViewModel.Username,
            Type = secretCreateViewModel.Type,
            Content = encryptedContent
        };
        
        try
        {
            await httpApiService.PostAsync<SecretDetailsResponse>(BaseUrl, secretRequest);
        }
        catch (ApiRequestUnauthorizedException)
        {
            await authenticationStateProvider.Logout();
        }
    }

    public async Task UpdateBatch(SecretUpdateBatchRequest batchUpdateBatchRequest)
    {
        try
        {
            await httpApiService.PutAsync<IList<SecretDetailsResponse>>(BatchUpdateUlr, batchUpdateBatchRequest);
        }
        catch (ApiRequestUnauthorizedException)
        {
            await authenticationStateProvider.Logout();
        }
    }

    public async Task DeleteSecret(string id)
    {
        try
        {
            await httpApiService.DeleteAsync($"{BaseUrl}/{id}");
        }
        catch (ApiRequestUnauthorizedException)
        {
            await authenticationStateProvider.Logout();
        }
    }

    public async Task<string> DecryptSecretContent(string content)
    {
        var masterPassword = await masterPasswordStorage.GetMasterPassword();
        if (string.IsNullOrEmpty(masterPassword))
            throw new AuthenticationException("Master password not set!");
        var decryptedContent = await encryptionService.Decrypt(content, masterPassword);

        return decryptedContent;
    }
    
    public async Task ReencryptAllSecrets(string oldMasterPassword, string newMasterPassword)
    {
        try
        {
            var secrets = await ListSecrets();
            if (!secrets.Any())
                return;
        
            var reencryptedSecrets = new List<SecretUpdateRequest>();
            foreach (var secret in secrets)
            {
                var secretDetails = await GetSecret(secret.Id);
                if (secretDetails == null)
                    continue;

                var decryptedContent = await encryptionService.Decrypt(secretDetails.Content, oldMasterPassword);
                var reencryptedContent = await encryptionService.Encrypt(decryptedContent, newMasterPassword);
                var secretUpdateRequest = new SecretUpdateRequest
                {
                    Id = secret.Id,
                    Name = secretDetails.Name,
                    Description = secretDetails.Description,
                    Username = secretDetails.Username,
                    Type = secretDetails.Type,
                    Content = reencryptedContent
                };
                reencryptedSecrets.Add(secretUpdateRequest);
            }

            var batchUpdateRequest = new SecretUpdateBatchRequest { Secrets = reencryptedSecrets };
            await UpdateBatch(batchUpdateRequest);
        }
        catch (ApiRequestUnauthorizedException)
        {
            await authenticationStateProvider.Logout();
        }
    }

    public Task<IList<SecretTypeViewModel>> GetSecretTypes()
    {
        var secretTypes = new List<SecretTypeViewModel>()
        {
            new("Nøgle", nameof(SecretType.Key)),
            new("Konto", nameof(SecretType.Account)),
            new("Kort", nameof(SecretType.Card)),
            new("Note", nameof(SecretType.Note)),
            new("Andet", nameof(SecretType.Other))
        };

        return Task.FromResult<IList<SecretTypeViewModel>>(secretTypes);
    }
}
