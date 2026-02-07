using System;
using System.IO;
using Xunit;
using a4crypt;

namespace Tests
{
    public class A4CryptFileTests
    {
        private static byte[] Rand(int size)
        {
            var b = new byte[size];
            Random.Shared.NextBytes(b);
            return b;
        }

        private static string TempFile()
            => Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

        [Fact]
        public void SaveAndOpen_RoundTrip_Succeeds()
        {
            var path = TempFile();

            var nonce = Rand(G.NonceSize);
            var salt = Rand(G.SaltSize);
            var tag = Rand(G.TagSize);
            var contents = Rand(1024);

            A4CryptFile.Save(
                path,
                nonce,
                salt,
                tag,
                G.KeyTypes.PBKDF2,
                G.KeyStrengths.High,
                contents
            );

            var parsed = A4CryptFile.Open(path);

            Assert.Equal(nonce, parsed.Nonce);
            Assert.Equal(salt, parsed.Salt);
            Assert.Equal(tag, parsed.Tag);
            Assert.Equal(contents, parsed.Contents);
            Assert.Equal(G.KeyTypes.PBKDF2, parsed.KeyType);
            Assert.Equal(G.KeyStrengths.High, parsed.KeyStrength);
        }
        [Fact]
        public void Open_InvalidMagic_Throws()
        {
            var path = TempFile();
            File.WriteAllBytes(path, Rand(64));

            Assert.Throws<FileInterfaceException>(() =>
                A4CryptFile.Open(path));
        }

        [Fact]
        public void Open_InvalidVersion_Throws()
        {
            var path = TempFile();
            var data = new byte[G.FileContentsStart];

            Array.Copy(G.ExpectedMagicSequence, data, 4);
            data[G.VersionStart] = 0xFF; // wrong version

            File.WriteAllBytes(path, data);

            Assert.Throws<FileInterfaceException>(() =>
                A4CryptFile.Open(path));
        }
        [Fact]
        public void Open_InvalidKeyType_Throws()
        {
            var path = TempFile();

            var nonce = Rand(G.NonceSize);
            var salt = Rand(G.SaltSize);
            var tag = Rand(G.TagSize);

            using (var stream = File.Open(path, FileMode.CreateNew))
            {
                stream.Write(G.ExpectedMagicSequence);
                stream.Write([G.ExpectedVersion]);
                stream.Write(nonce);
                stream.Write(salt);
                stream.Write(tag);
                stream.WriteByte(0xFF); // invalid key type
                stream.WriteByte((byte)G.KeyStrengths.Low);
                stream.Write(Rand(10));
            }

            Assert.Throws<FileInterfaceException>(() =>
                A4CryptFile.Open(path));
        }

        [Fact]
        public void Open_InvalidKeyStrength_Throws()
        {
            var path = TempFile();

            var nonce = Rand(G.NonceSize);
            var salt = Rand(G.SaltSize);
            var tag = Rand(G.TagSize);

            using (var stream = File.Open(path, FileMode.CreateNew))
            {
                stream.Write(G.ExpectedMagicSequence);
                stream.Write([G.ExpectedVersion]);
                stream.Write(nonce);
                stream.Write(salt);
                stream.Write(tag);
                stream.WriteByte((byte)G.KeyTypes.PBKDF2);
                stream.WriteByte(0xFF); // invalid strength
                stream.Write(Rand(10));
            }

            Assert.Throws<FileInterfaceException>(() =>
                A4CryptFile.Open(path));
        }
        [Fact]
        public void Open_TruncatedFile_Throws()
        {
            var path = TempFile();
            File.WriteAllBytes(path, new byte[10]); // too small

            Assert.ThrowsAny<Exception>(() =>
                A4CryptFile.Open(path));
        }
        [Fact]
        public void Constructor_InvalidSizes_Throws()
        {
            Assert.Throws<FileInterfaceException>(() =>
                new A4CryptFile(
                    new byte[1],
                    new byte[G.SaltSize],
                    new byte[G.TagSize],
                    (int)G.KeyTypes.PBKDF2,
                    (int)G.KeyStrengths.Low,
                    Array.Empty<byte>()
                ));
        }

    }
}