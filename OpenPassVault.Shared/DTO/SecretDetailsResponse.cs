namespace OpenPassVault.Shared.DTO;

public record SecretDetailsResponse(
    string Id, 
    string Name, 
    string Description, 
    string Username,
    string Type,
    string Content,
    string Created,
    string Updated);
    