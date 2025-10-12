using OpenPassVault.Shared.DTO;

namespace OpenPassVault.Web.Services.Interfaces;

public interface ICaptchaApiService
{
    Task<NewCaptchaResponse> GetNewCaptcha();
}