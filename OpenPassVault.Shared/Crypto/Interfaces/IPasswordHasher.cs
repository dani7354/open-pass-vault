namespace OpenPassVault.Shared.Crypto.Interfaces;

public interface IPasswordHasher
{
    Task<string> HashPassword(string password);
    Task<bool> VerifyPassword(string hashedPassword, string password);
}