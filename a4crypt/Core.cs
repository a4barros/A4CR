using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace a4crypt
{
    internal class Core
    {
        private const int Iterations = G.Iterations;
        private const int KeySize = G.KeySize;
        public static byte[] DeriveKey(string password, byte[] salt)
        {
            return Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);
        }
        private static byte[] GenSalt()
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }
        private static byte[] GenNonce()
        {
            byte[] salt = new byte[12];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }
        public static void Encrypt(string inputPath, string outputPath, string password)
        {
            byte[] salt = GenSalt();
            byte[] nonce = GenNonce();
            byte[] key = DeriveKey(password, salt);
            byte[] tag = new byte[G.TagSize];

            var stream = File.Open(inputPath, FileMode.Open, FileAccess.Read);

            byte[] output = new byte[stream.Length];
            byte[] fileContents = new byte[stream.Length];

            using AesGcm aes = new AesGcm(key, G.TagSize);
            stream.ReadExactly(fileContents);
            aes.Encrypt(nonce, fileContents, output, tag);
            CryptFileInterface.Save(outputPath, nonce, salt, tag, fileContents);
        }
        public static void Decrypt(string inputPath, string outputPath, string password)
        {
            var encryptedFile = CryptFileInterface.Open(inputPath);
            byte[] key = DeriveKey(password, encryptedFile.Salt);
            using AesGcm aes = new AesGcm(key, G.TagSize);
            byte[] output = new byte[encryptedFile.Contents.Length];
            aes.Decrypt(encryptedFile.Nonce, encryptedFile.Contents, encryptedFile.Tag, output);
            File.WriteAllBytes(outputPath, output);
        }
    }
}
