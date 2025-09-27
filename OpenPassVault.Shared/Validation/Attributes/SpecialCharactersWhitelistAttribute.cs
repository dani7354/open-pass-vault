using System.ComponentModel.DataAnnotations;

namespace OpenPassVault.Shared.Validation.Attributes;

public class SpecialCharactersWhitelistAttribute(HashSet<char> allowedSpecialCharacters) : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        var stringValue = value as string;
        if (!string.IsNullOrEmpty(stringValue))
        {
            var invalidChars = string.Join(", ", stringValue.Where(c => !IsCharValid(c)));
            if (!string.IsNullOrEmpty(invalidChars))
            {
                ErrorMessage = $"Feltet kan ikke indeholde: {invalidChars}";
                return false;
            }
        }
        
        return true; // this validator accepts null
    }

    protected virtual bool IsCharValid(char c) => char.IsLetterOrDigit(c) || allowedSpecialCharacters.Contains(c);
}