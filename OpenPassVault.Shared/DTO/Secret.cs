
namespace OpenPassVault.Shared.DTO;

public class Secret
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Comment { get; set; }
}
