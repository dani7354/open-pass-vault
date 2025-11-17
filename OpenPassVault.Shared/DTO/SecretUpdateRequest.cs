using System.ComponentModel.DataAnnotations;
using OpenPassVault.Shared.Validation;
using OpenPassVault.Shared.Validation.Attributes;

namespace OpenPassVault.Shared.DTO;

public class SecretUpdateRequest
{
    [Required(ErrorMessage = ErrorMessages.Required), 
     MaxLength(SecretFieldLengths.IdMaxLength, ErrorMessage = ErrorMessages.MaxLength)]
    [ValidGuidFormat]
    public string Id { get; set; } = null!;
    
    [Required(ErrorMessage = ErrorMessages.Required), 
     MaxLength(SecretFieldLengths.NameMaxLength, ErrorMessage = ErrorMessages.MaxLength)] 
    [SecretNameValidChars]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = ErrorMessages.Required), 
     MaxLength(SecretFieldLengths.TypeMaxLength, ErrorMessage = ErrorMessages.MaxLength)]
    [ValidSecretType]
    public string Type { get; set; } = null!;

    [Required(ErrorMessage = ErrorMessages.Required), 
     MaxLength(SecretFieldLengths.ContentMaxLength, ErrorMessage = ErrorMessages.MaxLength)]
    [ValidBase64String]
    public string Content { get; set; } = null!;
    
    [MaxLength(SecretFieldLengths.UsernameMaxLength, ErrorMessage = ErrorMessages.MaxLength)]
    [SecretUsernameValidChars]
    public string? Username { get; set; }

    [MaxLength(SecretFieldLengths.DescriptionMaxLength, ErrorMessage = ErrorMessages.MaxLength)]
    [SecretDescriptionValidChars]
    public string? Description { get; set; }
}