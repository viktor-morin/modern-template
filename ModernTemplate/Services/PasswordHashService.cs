using System.Security.Cryptography;

namespace ModernTemplate.Services;

public class PasswordHashService : IPasswordHashService
{
    private const int SaltSize = 16; //https://en.wikipedia.org/wiki/PBKDF2
    private const int HashSize = 32;
    private const int Iterations = 210_000; //https://en.wikipedia.org/wiki/PBKDF2

    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA512;

    public string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);

        return $"{Convert.ToHexString(hash)}.{Convert.ToHexString(salt)}";
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        var parts = passwordHash.Split('.');
        var hash = Convert.FromHexString(parts[0]);
        var salt = Convert.FromHexString(parts[1]);

        var passwordHashAttempt = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);
        return CryptographicOperations.FixedTimeEquals(hash, passwordHashAttempt);
    }
}


public interface IPasswordHashService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}