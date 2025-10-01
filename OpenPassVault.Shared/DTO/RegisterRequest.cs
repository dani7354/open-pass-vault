using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;
using OpenPassVault.Shared.Validation;
using OpenPassVault.Shared.Validation.Attributes;

namespace OpenPassVault.Shared.DTO;

public class RegisterRequest
{
    [Required(ErrorMessage = ErrorMessages.Required), 
     EmailAddress(ErrorMessage = ErrorMessages.EmailAddress), 
     MaxLength(RegisterFieldLengths.EmailMaxLength, ErrorMessage = ErrorMessages.MaxLength)]
    [UserEmailValidChars]
    public string Email { get; init; } = null!;

    [Required(ErrorMessage = ErrorMessages.Required), 
     MinLength(RegisterFieldLengths.PasswordMinLength, ErrorMessage = ErrorMessages.MinLength), 
     MaxLength(RegisterFieldLengths.PasswordMaxLength, ErrorMessage = ErrorMessages.MaxLength)]
    public string Password { get; init; } = null!;

    [Required(ErrorMessage = ErrorMessages.Required), 
     Compare(nameof(Password), ErrorMessage = ErrorMessages.Compare)] 
    public string ConfirmPassword { get; set; } = null!;

    [Required(ErrorMessage = ErrorMessages.Required), 
     MaxLength(RegisterFieldLengths.MasterPasswordHashMaxLength, ErrorMessage = ErrorMessages.MaxLength)] 
    public string MasterPasswordHash { get; init; } = null!;
}