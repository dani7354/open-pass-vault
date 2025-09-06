using System.ComponentModel.DataAnnotations;

namespace OpenPassVault.Shared.DTO;

public class RegisterRequest
{
    [Required, MaxLength(256)]
    public string? Name { get; set; }
    
    [Required, EmailAddress, MaxLength(256)]
    public string? Email { get; set; }
    
    [Required, MinLength(12), MaxLength(256)]
    public string? Password { get; set; }
    
    [Required, Compare(nameof(Password))]
    public string? ConfirmPassword { get; set; }
    
    [Required, MinLength(16), MaxLength(256)]
    public string? MasterPassword { get; set; }
    
    [Required, Compare(nameof(Password))]
    public string? ConfirmMasterPassword { get; set; }
}