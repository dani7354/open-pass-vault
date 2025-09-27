namespace OpenPassVault.Shared.Validation.Attributes;

public class SecretNameValidCharsAttribute() 
    : SpecialCharactersWhitelistAttribute(AllowedSpecialCharacters)
{
    private static readonly HashSet<char> AllowedSpecialCharacters = [
        '.', ',', ';', ':', '-', '_', '(', ')', '#', '@', ' '
    ];
}