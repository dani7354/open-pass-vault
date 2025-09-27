using System.ComponentModel.DataAnnotations;
using OpenPassVault.Shared.Validation;
using OpenPassVault.Shared.Validation.Attributes;

namespace OpenPassVault.Shared.DTO;

public class LoginRequest
{
    [Required, EmailAddress, MaxLength(LoginFieldLengths.EmailMaxLength), UserEmailValidChars]
    public string Email { get; set; } = null!;

    [Required, MinLength(LoginFieldLengths.PasswordMinLength), MaxLength(LoginFieldLengths.PasswordMaxLength)]
    public string Password { get; set; } = null!;
}