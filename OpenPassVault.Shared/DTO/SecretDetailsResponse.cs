using System.Text.Json.Serialization;

namespace OpenPassVault.Shared.DTO;


public class SecretDetailsResponse
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;
    
    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;
    
    [JsonPropertyName("description")]
    public string Description { get; init; } = null!;
    
    [JsonPropertyName("username")]
    public string Username { get; init; } = null!;
    
    [JsonPropertyName("type")]
    public string Type { get; init; } = null!;
    
    [JsonPropertyName("content")]
    public string Content { get; init; } = null!;
    
    [JsonPropertyName("created")]
    public string Created { get; init; } = null!;
    
    [JsonPropertyName("updated")]
    public string Updated { get; init; } = null!;
}
    