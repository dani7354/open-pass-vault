using System.ComponentModel.DataAnnotations;
using OpenPassVault.Shared.DTO;

namespace OpenPassVault.Shared.Validation.Attributes;

public class ValidSecretTypeAttribute : ValidationAttribute
{
    private readonly int _minValue;
    private readonly int _maxValue;

    private const string SecretTypeNotParsedErrorMessage = "Type ikke genkendt. Vælg venligt en anden!";
    
    public ValidSecretTypeAttribute()
    {
        var enumValues = Enum.GetValues<SecretType>().Select(x => (int) x).ToArray();
        _minValue = enumValues.Min();
        _maxValue = enumValues.Max();
    }
    public override bool IsValid(object? value)
    {
        var valueStr = value?.ToString();
        if (string.IsNullOrEmpty(valueStr))
        {
            ErrorMessage = "Feltet kan ikke være tomt";
            return false;
        }

        if (int.TryParse(valueStr, out var intValue) && (intValue < _minValue || intValue > _maxValue))
        {
            ErrorMessage = SecretTypeNotParsedErrorMessage;
            return false;
        }
        
        if (!Enum.TryParse<SecretType>(valueStr, out _))
        {
            ErrorMessage = SecretTypeNotParsedErrorMessage;
            return false;
        }

        return true;
    }
}