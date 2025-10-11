namespace OpenPassVault.Shared.DTO;

public class NewCaptchaResponse
{
    public string CaptchaImageBase64 { get; init; } = null!;
    public string CaptchaHmac { get; init; } = null!;
}