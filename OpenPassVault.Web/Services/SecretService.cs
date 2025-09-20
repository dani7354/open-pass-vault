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
    IEncryptionService encryptionService) : ISecretService
{
    private const string BaseUrl = "secrets";
    
    public async Task<IList<SecretListItemResponse>> ListSecrets()
    {
        try
        {
            var response = await httpApiService.GetAsync<IList<SecretListItemResponse>>(BaseUrl);
            return response ?? new List<SecretListItemResponse>();
        }
        catch (RequestUnauthorizedException)
        {
            await authenticationStateProvider.Logout();
            return new List<SecretListItemResponse>();
        }
    }

    public async Task CreateSecret(SecretCreateViewModel secretCreateViewModel)
    {
        var masterPassword = "aaaaaaaaaaaaaaaaaaaaaaa"; 
        var  encryptedContent =  await encryptionService.Encrypt(secretCreateViewModel.ContentPlain, masterPassword);
        var secretRequest = new SecretCreateRequest()
        {
            Name = secretCreateViewModel.Name,
            Description = secretCreateViewModel.Description,
            Username = secretCreateViewModel.Username,
            Type = secretCreateViewModel.Type,
            Content = encryptedContent
        };
        
        await httpApiService.PostAsync<string>(BaseUrl, secretRequest);
    }
}