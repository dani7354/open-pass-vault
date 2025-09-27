using System.ComponentModel.DataAnnotations;
using OpenPassVault.Shared.Validation;
using OpenPassVault.Shared.Validation.Attributes;
using OpenPassVault.Web.Pages;

namespace OpenPassVault.Web.Models;

public class RegisterViewModel
{
    [Required, EmailAddress, MaxLength(RegisterFieldLengths.EmailMaxLength), UserEmailValidChars]
    public string Email { get; set; } = null!;

    [Required, MinLength(RegisterFieldLengths.PasswordMinLength), MaxLength(RegisterFieldLengths.PasswordMaxLength)]
    public string Password { get; set; } = null!;

    [Required, Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = null!;

    [Required, MinLength(RegisterFieldLengths.MasterPasswordMinLength), MaxLength(RegisterFieldLengths.MasterPasswordMaxLength)]
    public string MasterPassword { get; set; } = null!;

    [Required, Compare(nameof(MasterPassword))]
    public string ConfirmMasterPassword { get; set; } = null!;
}