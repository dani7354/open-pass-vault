using System.Security;
using System.Text.Json.Serialization;

namespace OpenPassVault.Shared.DTO;

public class SecretListItemResponse
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;
    
    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;
    
    [JsonPropertyName("username")]
    public string Username { get; init; } = null!;
    
    [JsonPropertyName("type")]
    public string Type { get; init; } = null!;
    
    [JsonIgnore]
    public string? ContentDecrypted { get; set; }
}