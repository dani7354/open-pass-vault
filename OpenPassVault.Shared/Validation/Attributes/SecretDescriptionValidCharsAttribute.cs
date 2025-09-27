namespace OpenPassVault.Shared.Validation.Attributes;

public class SecretDescriptionValidCharsAttribute() 
    : SpecialCharactersWhitelistAttribute(ValidSpecialCharacters)
{
    private static readonly HashSet<char> ValidSpecialCharacters = [
        '.', ',', ';', ':', '-', '_', '(', ')', '=', '"', '\'', '/', '#', '@', ' '];
}