using OpenPassVault.Shared.DTO;

namespace OpenPassVault.API.Services.Interfaces;

public interface ICaptchaService
{
    Task<bool> VerifyCaptcha(string captchaResponse, string captchaHmac);
    Task<NewCaptchaResponse> GenerateCaptcha();
}