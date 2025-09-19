using System.Text.Json.Serialization;

namespace OpenPassVault.Shared.DTO;

public class TokenResponse
{
    [JsonPropertyName("token")]
    public string Token { get; init; } = null!;
}
