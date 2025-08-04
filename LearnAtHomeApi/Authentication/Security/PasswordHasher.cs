using System.Security.Cryptography;
using System.Text;
using LearnAtHomeApi.User.Dto;
using Microsoft.AspNetCore.Identity;

namespace LearnAtHomeApi.Authentication.Security;

public interface IPasswordHasher
{
    string Hash(string password);
    bool VerifyHashedPassword(string hashedPassword, string providedPassword);
}

internal sealed class PasswordHasher : IPasswordHasher
{
    /// <summary>
    /// Constants defined for V1 tokens
    /// </summary>
    private class V1Constants
    {
        internal const string VersionIdentifier = "V1";
        internal const int SaltSize = 16;
        internal const int HashSize = 32;
        internal const int Iterations = 100000;
    };

    private readonly HashAlgorithmName AlgorithmV1 = HashAlgorithmName.SHA512;

    /// <summary>
    /// Hash a string using the last version hasher constants
    /// </summary>
    /// <param name="password">String to hash</param>
    /// <returns>Hashed string (password and salt)</returns>
    public string Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(V1Constants.SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            V1Constants.Iterations,
            AlgorithmV1,
            V1Constants.HashSize
        );

        return $"{Convert.ToBase64String(hash)}-{Convert.ToBase64String(salt)}-{Convert.ToBase64String(Encoding.ASCII.GetBytes(V1Constants.VersionIdentifier))}";
    }

    /// <summary>
    /// Compares a hashed string with the given string.
    /// <br /> <br/>
    /// It points to other methods for each hasher version.
    /// <br />
    /// This is to ensure that if we use a new hash system, older passwords will still be parseable.
    /// </summary>
    /// <param name="hashedPassword">Already hashed string (password and salt)</param>
    /// <param name="providedPassword">String to hash and compare</param>
    /// <returns>True if the comparison was a success</returns>
    public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
    {
        string[] splitHash = hashedPassword.Split('-');
        byte[] hash = Convert.FromBase64String(splitHash[0]);
        byte[] salt = Convert.FromBase64String(splitHash[1]);
        string version = Encoding.ASCII.GetString(Convert.FromBase64String(splitHash[2]));

        if (version == V1Constants.VersionIdentifier)
            return VerifyHashedPasswordV1(
                hash,
                salt,
                providedPassword,
                V1Constants.HashSize,
                V1Constants.Iterations,
                AlgorithmV1
            );

        return false;
    }

    /// <summary>
    /// Compares a hashed string with the given string using V1 hasher constants.
    /// </summary>
    /// <param name="hash">Already hashed password</param>
    /// <param name="salt">Salt used to hash the password</param>
    /// <param name="providedPassword">Given string to compare with</param>
    /// <param name="hashSize">Size of the original hash</param>
    /// <param name="iterations">Number of iterations of the original hash</param>
    /// <param name="algorithm">Algorythm used for the original hash</param>
    /// <returns>True if the comparison was a success</returns>
    private bool VerifyHashedPasswordV1(
        byte[] hash,
        byte[] salt,
        string providedPassword,
        int hashSize,
        int iterations,
        HashAlgorithmName algorithm
    )
    {
        byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(
            providedPassword,
            salt,
            iterations,
            algorithm,
            hashSize
        );
        return CryptographicOperations.FixedTimeEquals(inputHash, hash);
    }
}
