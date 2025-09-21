namespace OpenPassVault.Web.Models;

public record User(
    string Name, 
    string Email,
    string MasterPasswordHash);