using System.Text.Json.Serialization;

namespace OpenPassVault.Shared.DTO;

public class NewCaptchaResponse
{
    [JsonPropertyName("captchaImageBase64")]
    public string CaptchaImageBase64 { get; init; } = null!;
    [JsonPropertyName("captchaHmac")]
    public string CaptchaHmac { get; init; } = null!;
}