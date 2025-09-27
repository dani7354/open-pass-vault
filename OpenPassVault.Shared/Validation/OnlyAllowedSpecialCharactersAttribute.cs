using System.ComponentModel.DataAnnotations;

namespace OpenPassVault.Shared.Validation;

public class OnlyAllowedSpecialCharactersAttribute : ValidationAttribute
{
    private static HashSet<char> _allowedSpecialCharacters =
        ['.', ',', ';', ':', '-', '_', '(', ')', '=', '"', '\'', '/', '#', '@'];
    
    public override bool IsValid(object? value)
    {
        var stringValue = value as string;
        if (!string.IsNullOrEmpty(stringValue))
        {
            var invalidChars = string.Join(", ", stringValue.Where(c => !IsCharValid(c)));
            if (string.IsNullOrEmpty(invalidChars))
            {
                ErrorMessage = $"Feltet kan ikke indeholde: {invalidChars}";
                return false;
            }
        }
        
        return true; // this validator accepts null
    }

    private bool IsCharValid(char c) => char.IsLetterOrDigit(c) || _allowedSpecialCharacters.Contains(c);
}