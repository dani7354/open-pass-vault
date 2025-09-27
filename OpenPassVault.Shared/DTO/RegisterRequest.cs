using System.ComponentModel.DataAnnotations;
using OpenPassVault.Shared.Validation;
using OpenPassVault.Shared.Validation.Attributes;

namespace OpenPassVault.Shared.DTO;

public class RegisterRequest
{
    [Required, EmailAddress, MaxLength(RegisterFieldLengths.EmailMaxLength), UserEmailValidChars]
    public string Email { get; init; } = null!;

    [Required, MinLength(RegisterFieldLengths.PasswordMinLength), MaxLength(RegisterFieldLengths.PasswordMaxLength)]
    public string Password { get; init; } = null!;

    [Required, Compare(nameof(Password))] 
    public string ConfirmPassword { get; set; } = null!;

    [Required, MaxLength(RegisterFieldLengths.MasterPasswordHashMaxLength)] 
    public string MasterPasswordHash { get; init; } = null!;
}