using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace FullStackAssignment.Application.Mappers
{
    public static class PasswordHelper
    {
        public static string ToHashedPassword(string password)
        {
            // generate a 128-bit salt using a secure PRNG
            byte[] salt = RandomNumberGenerator.GetBytes(16);

            // derive a 256-bit subkey (use HMACSHA256 with 10,000 iterations)
            byte[] hashed = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32);

            // store as: {salt}.{hash}
            return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hashed)}";
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            var parts = hashedPassword.Split('.');
            if (parts.Length != 2) return false;

            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] storedHash = Convert.FromBase64String(parts[1]);

            // hash incoming password with same salt
            byte[] hashed = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32);

            // compare securely
            return CryptographicOperations.FixedTimeEquals(storedHash, hashed);
        }
    }
}
