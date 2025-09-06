using System.ComponentModel.DataAnnotations;

namespace OpenPassVault.Shared.DTO;

public class LoginRequest
{
    [Required, EmailAddress, MaxLength(256)]
    public string Email { get; set; } = null!;

    [Required, MinLength(16), MaxLength(256)]
    public string Password { get; set; } = null!;
}