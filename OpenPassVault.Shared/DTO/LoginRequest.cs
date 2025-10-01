using System.ComponentModel.DataAnnotations;
using OpenPassVault.Shared.Validation;
using OpenPassVault.Shared.Validation.Attributes;

namespace OpenPassVault.Shared.DTO;

public class LoginRequest
{
    [Required(ErrorMessage = ErrorMessages.Required), 
     EmailAddress(ErrorMessage = ErrorMessages.EmailAddress), 
     MaxLength(LoginFieldLengths.EmailMaxLength), 
     UserEmailValidChars]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = ErrorMessages.Required), 
     MinLength(LoginFieldLengths.PasswordMinLength, ErrorMessage = ErrorMessages.MinLength), 
     MaxLength(LoginFieldLengths.PasswordMaxLength, ErrorMessage = ErrorMessages.MaxLength)]
    public string Password { get; set; } = null!;
}