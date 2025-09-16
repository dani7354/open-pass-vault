using System.ComponentModel.DataAnnotations;

namespace OpenPassVault.Shared.DTO;

public class RegisterRequest
{
    [Required, EmailAddress, MaxLength(256)]
    public string Email { get; set; } = null!;

    [Required, MinLength(12), MaxLength(256)]
    public string Password { get; set; } = null!;

    [Required, Compare(nameof(Password))] 
    public string ConfirmPassword { get; set; } = null!;

    [Required, MaxLength(256)] 
    public string MasterPasswordHash { get; set; } = null!;
}