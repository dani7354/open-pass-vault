using System.ComponentModel.DataAnnotations;
using OpenPassVault.Shared.Validation;

namespace OpenPassVault.Shared.DTO;

public class LoginRequest
{
    [Required, EmailAddress, MaxLength(LoginFieldLengths.EmailMaxLength)]
    public string Email { get; set; } = null!;

    [Required, MinLength(LoginFieldLengths.PasswordMinLength), MaxLength(LoginFieldLengths.PasswordMaxLength)]
    public string Password { get; set; } = null!;
}