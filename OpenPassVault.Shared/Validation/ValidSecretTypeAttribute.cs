using System.ComponentModel.DataAnnotations;
using OpenPassVault.Shared.DTO;

namespace OpenPassVault.Shared.Validation;

public class ValidSecretTypeAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        var valueStr = value?.ToString();
        if (string.IsNullOrEmpty(valueStr))
        {
            ErrorMessage = "Feltet kan ikke være tomt";
            return false;
        }
        
        if (!Enum.TryParse<SecretType>(value as string, out _))
        {
            ErrorMessage = "Type ikke genkendt. Vælg venligt en anden!";
            return false;
        }

        return true;
    }
}