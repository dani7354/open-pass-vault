using OpenPassVault.Shared.DTO;
using OpenPassVault.Web.Services.Interfaces;

namespace OpenPassVault.Web.Services;

public class CaptchaApiService(IHttpApiService httpApiService) : ICaptchaApiService
{
    private const string CaptchaBaseUrl = "captcha";
    private const string NewCaptchaUrl = $"{CaptchaBaseUrl}/new";
    
    public async Task<NewCaptchaResponse> GetNewCaptcha()
    {
        var response = await httpApiService.GetAsync<NewCaptchaResponse>(NewCaptchaUrl);
        if (response == null)
            throw new Exception("Failed to get new captcha from API.");
        
        return response;
    }
}