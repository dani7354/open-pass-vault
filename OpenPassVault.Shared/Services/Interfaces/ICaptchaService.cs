using OpenPassVault.Shared.DTO;

namespace OpenPassVault.Shared.Services.Interfaces;

public interface ICaptchaService
{
    Task<bool> VerifyCaptcha(string captchaResponse, string captchaHmac);
    bool CaptchaFormatIsValid(string captchaCode);
    Task<NewCaptchaResponse> GenerateCaptcha();
}