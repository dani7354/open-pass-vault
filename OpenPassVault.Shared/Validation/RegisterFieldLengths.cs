namespace OpenPassVault.Shared.Validation;

public static class RegisterFieldLengths
{
    public const int EmailMaxLength = 256;
    public const int PasswordMinLength = 16;
    public const int  PasswordMaxLength = 256;
    public const int MasterPasswordMinLength = 16;
    public const int MasterPasswordMaxLength = 256;
    public const int MasterPasswordHashMaxLength = 100;
}