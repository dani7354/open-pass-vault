using System.ComponentModel.DataAnnotations;
using OpenPassVault.Shared.Validation;
using OpenPassVault.Shared.Validation.Attributes;
using OpenPassVault.Web.Pages;

namespace OpenPassVault.Web.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = ErrorMessages.Required), 
     EmailAddress(ErrorMessage = ErrorMessages.EmailAddress), 
     MaxLength(RegisterFieldLengths.EmailMaxLength, ErrorMessage = ErrorMessages.MaxLength)]
    [UserEmailValidChars]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = ErrorMessages.Required), 
     MinLength(RegisterFieldLengths.PasswordMinLength, ErrorMessage = ErrorMessages.MinLength),
     MaxLength(RegisterFieldLengths.PasswordMaxLength, ErrorMessage = ErrorMessages.MaxLength)]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = ErrorMessages.Required), 
     Compare(nameof(Password), ErrorMessage = ErrorMessages.Compare)]
    public string ConfirmPassword { get; set; } = null!;

    [Required(ErrorMessage = ErrorMessages.Required), 
     MinLength(RegisterFieldLengths.MasterPasswordMinLength, ErrorMessage = ErrorMessages.MinLength), 
     MaxLength(RegisterFieldLengths.MasterPasswordMaxLength, ErrorMessage = ErrorMessages.MaxLength)]
    public string MasterPassword { get; set; } = null!;

    [Required(ErrorMessage = ErrorMessages.Required), 
     Compare(nameof(MasterPassword), ErrorMessage = ErrorMessages.Compare)]
    public string ConfirmMasterPassword { get; set; } = null!;

    [Required(ErrorMessage = ErrorMessages.Required)]
    public string CaptchaCode { get; set; } = null!;
    

    public string CaptchaHmac { get; init; } = null!;
    
    public string CaptchaImageSrc { get; init; } = null!;
    
    
}