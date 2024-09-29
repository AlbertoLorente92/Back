using Back.Interfaces;
using Back.Models;
using System.Security.Cryptography;

namespace Back.Implementation
{
    public class PasswordHashService : IPasswordHashService
    {
        private const int SALT_BYTES_SIZE = 16;
        private const int ITERATIONS = 10000;
        private const int HASH_BYTES_SIZE = 32;
        public HashPasswordResponse HashPassword(string plainPassword)
        {
            byte[] saltBytes = new byte[SALT_BYTES_SIZE];
            RandomNumberGenerator.Fill(saltBytes);

            using var pbkdf2 = new Rfc2898DeriveBytes(plainPassword, saltBytes, ITERATIONS, HashAlgorithmName.SHA256);
            byte[] hashBytes = pbkdf2.GetBytes(HASH_BYTES_SIZE);
            string salt = Convert.ToBase64String(saltBytes);
            string hashedPassword = Convert.ToBase64String(hashBytes);

            return new HashPasswordResponse() { HashedPassword = hashedPassword, Salt = salt };
        }

        public bool VerifyPassword(string password, string hashedPassword, string salt)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);
            using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, ITERATIONS, HashAlgorithmName.SHA256);
            byte[] hashBytes = pbkdf2.GetBytes(HASH_BYTES_SIZE); 
            string newHashedPassword = Convert.ToBase64String(hashBytes);
            return newHashedPassword == hashedPassword;
        }
    }
}
