using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace a4crypt
{
    internal class Core
    {
        private const int KeySize = G.KeySize;
        public static byte[] DeriveKey(string password, byte[] salt, G.KeyTypes keyType, G.KeyStrengths keyStrength)
        {
            int idx = (int)keyStrength;
            if ((uint)idx >= 4)
            {
                throw new CryptographicException("Invalid key strength");
            }

            switch (keyType)
            {
                case G.KeyTypes.PBKDF2:
                {
                    var iter = G.Pbkdf2Strengths[(int)keyStrength].Iterations;
                    return Rfc2898DeriveBytes.Pbkdf2(password, salt, iter, HashAlgorithmName.SHA256, KeySize);
                }
                case G.KeyTypes.Argon2id:
                {
                    var time = G.Argon2idStrengths[(int)keyStrength].Time;
                    var memoryMB = G.Argon2idStrengths[(int)keyStrength].MemoryMB;
                    var parallelism = G.Argon2idStrengths[(int)keyStrength].Parallelism;
                    using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
                    {
                        Salt = salt,
                        DegreeOfParallelism = parallelism,
                        MemorySize = memoryMB * 1024,
                        Iterations = time,
                    };
                    return argon2.GetBytes(G.KeySize);
                }

            }
            return [];
        }
        private static byte[] GenSalt()
        {
            byte[] salt = new byte[16];
            RandomNumberGenerator.Fill(salt);
            return salt;
        }
        private static byte[] GenNonce()
        {
            byte[] nonce = new byte[12];
            RandomNumberGenerator.Fill(nonce);
            return nonce;
        }
        public static void Encrypt(string inputPath, string outputPath, string password,
            G.KeyTypes keyType=G.KeyTypes.Argon2id, G.KeyStrengths keyStrength=G.KeyStrengths.High)
        {
            byte[] salt = GenSalt();
            byte[] nonce = GenNonce();
            byte[] key = DeriveKey(password, salt, keyType, keyStrength);
            byte[] tag = new byte[G.TagSize];

            var stream = File.Open(inputPath, FileMode.Open, FileAccess.Read);

            var aad = GenAAD(salt);

            byte[] output = new byte[stream.Length];
            byte[] fileContents = new byte[stream.Length];

            using AesGcm aes = new AesGcm(key, G.TagSize);
            stream.ReadExactly(fileContents);
            aes.Encrypt(nonce, fileContents, output, tag, aad);
            CryptographicOperations.ZeroMemory(key);
            CryptFileParser.Save(outputPath, nonce, salt, tag, keyType, keyStrength, output);
        }

        private static byte[] GenAAD(byte[] salt)
        {
            var aad = new byte[G.MagicSize + G.VersionSize + G.SaltSize];
            Buffer.BlockCopy(G.ExpectedMagicSequence, 0, aad, 0, G.MagicSize);
            aad[G.MagicSize] = (byte)G.ExpectedVersion;
            Buffer.BlockCopy(salt, 0, aad, G.MagicSize + G.VersionSize, G.SaltSize);
            return aad;
        }

        public static void Decrypt(string inputPath, string outputPath, string password,
            G.KeyTypes keyType = G.KeyTypes.Argon2id, G.KeyStrengths keyStrength = G.KeyStrengths.High)
        {
            var encryptedFile = CryptFileParser.Open(inputPath);
            byte[] key = DeriveKey(password, encryptedFile.Salt, keyType, keyStrength);
            using AesGcm aes = new AesGcm(key, G.TagSize);
            byte[] output = new byte[encryptedFile.Contents.Length];
            var aad = GenAAD(encryptedFile.Salt);
            aes.Decrypt(encryptedFile.Nonce, encryptedFile.Contents, encryptedFile.Tag, output, aad);
            CryptographicOperations.ZeroMemory(key);
            File.WriteAllBytes(outputPath, output);
        }
    }
}
