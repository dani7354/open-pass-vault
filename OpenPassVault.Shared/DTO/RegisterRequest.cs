using System.ComponentModel.DataAnnotations;

namespace OpenPassVault.Shared.DTO;

public class RegisterRequest
{
    [Required, EmailAddress, MaxLength(256)]
    public string? Email { get; set; }
    
    [Required, MinLength(16), MaxLength(256)]
    public string? Password { get; set; }
    
    [Required, Compare(nameof(Password))]
    public string? ConfirmPassword { get; set; }
}