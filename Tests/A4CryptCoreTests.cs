using System;
using System.IO;
using System.Security.Cryptography;
using Xunit;
using a4crypt;

namespace Tests
{
    public class A4CryptCoreTests
    {
        private static string TempFile()
            => Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

        private static byte[] RandomBytes(int size)
        {
            var b = new byte[size];
            RandomNumberGenerator.Fill(b);
            return b;
        }
        [Fact]
        public void EncryptDecrypt_RoundTrip_Argon2_Succeeds()
        {
            var input = TempFile();
            var encrypted = TempFile();
            var output = TempFile();

            var plaintext = RandomBytes(4096);
            File.WriteAllBytes(input, plaintext);

            A4CryptCore.Encrypt(
                input,
                encrypted,
                "correct horse battery staple",
                G.KeyTypes.Argon2id,
                G.KeyStrengths.Medium
            );

            A4CryptCore.Decrypt(
                encrypted,
                output,
                "correct horse battery staple"
            );

            var decrypted = File.ReadAllBytes(output);
            Assert.Equal(plaintext, decrypted);
        }
        [Fact]
        public void EncryptDecrypt_RoundTrip_PBKDF2_Succeeds()
        {
            var input = TempFile();
            var encrypted = TempFile();
            var output = TempFile();

            var plaintext = RandomBytes(1024);
            File.WriteAllBytes(input, plaintext);

            A4CryptCore.Encrypt(
                input,
                encrypted,
                "password123",
                G.KeyTypes.PBKDF2,
                G.KeyStrengths.High
            );

            A4CryptCore.Decrypt(
                encrypted,
                output,
                "password123"
            );

            Assert.Equal(plaintext, File.ReadAllBytes(output));
        }
        [Fact]
        public void Decrypt_WrongPassword_Throws()
        {
            var input = TempFile();
            var encrypted = TempFile();
            var output = TempFile();

            File.WriteAllBytes(input, RandomBytes(512));

            A4CryptCore.Encrypt(
                input,
                encrypted,
                "correct-password"
            );

            Assert.Throws<CryptographicException>(() =>
                A4CryptCore.Decrypt(
                    encrypted,
                    output,
                    "wrong-password"
                ));
        }
        [Fact]
        public void Decrypt_TamperedCiphertext_Throws()
        {
            var input = TempFile();
            var encrypted = TempFile();
            var output = TempFile();

            File.WriteAllBytes(input, RandomBytes(512));
            A4CryptCore.Encrypt(input, encrypted, "secret");

            var data = File.ReadAllBytes(encrypted);
            data[^1] ^= 0x01; // flip last bit
            File.WriteAllBytes(encrypted, data);

            Assert.Throws<CryptographicException>(() =>
                A4CryptCore.Decrypt(encrypted, output, "secret"));
        }
        [Fact]
        public void Decrypt_TamperedTag_Throws()
        {
            var input = TempFile();
            var encrypted = TempFile();
            var output = TempFile();

            File.WriteAllBytes(input, RandomBytes(256));
            A4CryptCore.Encrypt(input, encrypted, "secret");

            var data = File.ReadAllBytes(encrypted);

            // Flip a bit in the tag region
            int tagOffset = G.TagStart;
            data[tagOffset] ^= 0x01;

            File.WriteAllBytes(encrypted, data);

            Assert.Throws<CryptographicException>(() =>
                A4CryptCore.Decrypt(encrypted, output, "secret"));
        }
        [Fact]
        public void Decrypt_TamperedKeyStrength_Throws()
        {
            var input = TempFile();
            var encrypted = TempFile();
            var output = TempFile();

            File.WriteAllBytes(input, RandomBytes(128));
            A4CryptCore.Encrypt(
                input,
                encrypted,
                "secret",
                G.KeyTypes.Argon2id,
                G.KeyStrengths.High
            );

            var data = File.ReadAllBytes(encrypted);
            data[G.KeyStrengthStart] = (byte)G.KeyStrengths.Low;
            File.WriteAllBytes(encrypted, data);

            Assert.Throws<CryptographicException>(() =>
                A4CryptCore.Decrypt(encrypted, output, "secret"));
        }
    }
}