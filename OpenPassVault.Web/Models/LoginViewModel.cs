using System.ComponentModel.DataAnnotations;
using OpenPassVault.Shared.Validation;
using OpenPassVault.Shared.Validation.Attributes;

namespace OpenPassVault.Web.Models;

public class LoginViewModel
{
    [Required, EmailAddress, MaxLength(LoginFieldLengths.EmailMaxLength), UserEmailValidChars]
    public string Email { get; set; } = null!;

    [Required, MinLength(LoginFieldLengths.PasswordMinLength), MaxLength(LoginFieldLengths.EmailMaxLength)]
    public string Password { get; set; } = null!;

    [Required, MinLength(LoginFieldLengths.MasterPasswordMinLength), MaxLength(LoginFieldLengths.MasterPasswordMaxLength)]
    public string MasterPassword { get; set; } = null!;
}