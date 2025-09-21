using System.ComponentModel.DataAnnotations;
using OpenPassVault.Shared.Validation;

namespace OpenPassVault.Web.Models;

public class LoginViewModel
{
    [Required, EmailAddress, MaxLength(LoginFieldLengths.EmailMaxLength)]
    public string Email { get; set; } = null!;

    [Required, MinLength(LoginFieldLengths.PasswordMinLength), MaxLength(LoginFieldLengths.EmailMaxLength)]
    public string Password { get; set; } = null!;

    [Required, MinLength(LoginFieldLengths.MasterPasswordMinLength), MaxLength(LoginFieldLengths.MasterPasswordMaxLength)]
    public string MasterPassword { get; set; } = null!;
}