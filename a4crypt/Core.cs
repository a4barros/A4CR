using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace a4crypt
{
    internal class Core
    {

        private const int Iterations = 600000;
        private const int KeySize = 32;
        public static byte[] DeriveKey(string password, byte[] salt)
        {
            return Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);
        }
        private static byte[] GenerateSalt()
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }
        private byte[] Encrypt(byte[] key, byte[] input)
        {
            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.GenerateIV();
            using var encryptor = aes.CreateEncryptor();
            using var memoryStream = 



        }
        private byte[] Decrypt(byte[] key, byte[] input)
        {

        }
        public void EncryptFile(string inputPath, string outputPath, string password)
        {

        }
        public void DecryptFile(string inputPath, string outputPath, string password)
        {

        }
    }
}
