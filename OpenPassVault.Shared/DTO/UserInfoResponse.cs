using System.Text.Json.Serialization;

namespace OpenPassVault.Shared.DTO;

public class UserInfoResponse
{
    [JsonPropertyName("id")] 
    public string Id { get; init; } = null!;
    
    [JsonPropertyName("email")]
    public string Email { get; init; } = null!;
}