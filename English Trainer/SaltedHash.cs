using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace English_Trainer
{
    public class SaltedHash
    {
        public string Hash { get; private set; }
        // Соль — строка данных, которая передаётся хеш-функции вместе с паролем
        public string Salt { get; private set; }

        public SaltedHash(string password)
        {
            var saltBytes = new byte[32];
            new Random().NextBytes(saltBytes);
            Salt = Convert.ToBase64String(saltBytes);
            var passwordAndSaltBytes = Concat(password, saltBytes);
            Hash = ComputeHash(passwordAndSaltBytes);
        }

        static byte[] Concat(string password, byte[] saltBytes)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            return passwordBytes.Concat(saltBytes).ToArray();
        }

        static string ComputeHash(byte[] bytes)
        {
            using (var sha256 = SHA256.Create())
            {
                return Convert.ToBase64String(sha256.ComputeHash(bytes));
            }
        }

        public static bool Verify(string salt, string hash, string password)
        {
            var saltBytes = Convert.FromBase64String(salt);
            var passwordAndSaltBytes = Concat(password, saltBytes);
            var hashAttempt = ComputeHash(passwordAndSaltBytes);
            return hash == hashAttempt;
        }
    }
}
