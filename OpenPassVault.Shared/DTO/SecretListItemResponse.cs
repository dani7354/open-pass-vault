namespace OpenPassVault.Shared.DTO;

public class SecretListItemResponse
{
    public string Id { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string Username { get; init; } = null!;
    public string Type { get; init; } = null!;
}