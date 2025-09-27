namespace OpenPassVault.Shared.Validation.Attributes;

public sealed class UsernameValidCharsAttribute() : SpecialCharactersWhitelistAttribute(AllowedSpecialCharacters)
{
    private static readonly HashSet<char> AllowedSpecialCharacters = [ '.', '-', '_' ];
    protected override bool IsCharValid(char c) => char.IsAsciiLetterOrDigit(c) || AllowedSpecialCharacters.Contains(c);
}