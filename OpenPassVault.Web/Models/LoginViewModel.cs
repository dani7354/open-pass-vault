using System.ComponentModel.DataAnnotations;
using OpenPassVault.Shared.Validation;
using OpenPassVault.Shared.Validation.Attributes;

namespace OpenPassVault.Web.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = ErrorMessages.Required),
     EmailAddress(ErrorMessage = ErrorMessages.EmailAddress), 
     MaxLength(LoginFieldLengths.EmailMaxLength, ErrorMessage = ErrorMessages.MaxLength), 
     UserEmailValidChars]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = ErrorMessages.Required), 
     MinLength(LoginFieldLengths.PasswordMinLength, ErrorMessage = ErrorMessages.MinLength), 
     MaxLength(LoginFieldLengths.PasswordMaxLength, ErrorMessage = ErrorMessages.MaxLength)]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = ErrorMessages.Required), 
     MinLength(LoginFieldLengths.MasterPasswordMinLength, ErrorMessage = ErrorMessages.MinLength), 
     MaxLength(LoginFieldLengths.MasterPasswordMaxLength, ErrorMessage = ErrorMessages.MaxLength)]
    public string MasterPassword { get; set; } = null!;
}