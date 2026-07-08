using System;
using System.Security.Cryptography;
using Unison.LibraryManagement.Application.Security;

namespace Unison.LibraryManagement.Infrastructure.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        private readonly int _iterations;

        public PasswordHasher(int iterations = 100_000)
        {
            _iterations = iterations;
        }

        public PasswordHashResult Hash(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(16);
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, _iterations, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32);

            return new PasswordHashResult(Convert.ToBase64String(hash), Convert.ToBase64String(salt), _iterations);
        }

        public bool Verify(string password, string saltBase64, string hashBase64, int iterations)
        {
            var salt = Convert.FromBase64String(saltBase64);
            var expected = Convert.FromBase64String(hashBase64);
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            var actual = pbkdf2.GetBytes(expected.Length);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
    }
}
