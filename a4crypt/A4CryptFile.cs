using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace a4crypt
{
    public class A4CryptFile
    {
        public readonly byte[] Nonce;
        public readonly byte[] Salt;
        public byte[] Tag { get; }
        public byte[] Contents { get; }
        public G.KeyTypes KeyType { get; }
        public G.KeyStrengths KeyStrength { get; }
        public A4CryptFile(byte[] nonce, byte[] salt, byte[] tag, int keyType, int keyStrength, byte[] contents)
        {
            Nonce = nonce;
            Salt = salt;
            Tag = tag;
            Contents = contents;
            SanityCheck(nonce, salt, tag);

            if (!Enum.IsDefined(typeof(G.KeyTypes), keyType))
                throw new FileInterfaceException("Invalid key type");

            if (!Enum.IsDefined(typeof(G.KeyStrengths), keyStrength))
                throw new FileInterfaceException("Invalid key strength");

            KeyType = (G.KeyTypes)keyType;
            KeyStrength = (G.KeyStrengths)keyStrength;
        }
        public static A4CryptFile Open(string inputPath)
        {
            using var stream = File.Open(inputPath, FileMode.Open, FileAccess.Read);
            if (stream.Length > int.MaxValue)
                throw new CryptographicException("File too large");
            HeaderCheck(stream);

            stream.Position = G.NonceStart;
            var nonce = new byte[G.NonceSize];
            stream.ReadExactly(nonce);

            stream.Position = G.SaltStart;
            var salt = new byte[G.SaltSize];
            stream.ReadExactly(salt);

            stream.Position = G.TagStart;
            var tag = new byte[G.TagSize];
            stream.ReadExactly(tag);

            stream.Position = G.KeyTypeStart;
            int keytype = stream.ReadByte();

            stream.Position = G.KeyStrengthStart;
            int keystrength = stream.ReadByte();

            stream.Position = G.FileContentsStart;
            long fileLength = stream.Length - G.FileContentsStart;
            var fileContents = new byte[fileLength];
            stream.ReadExactly(fileContents);

            return new A4CryptFile(nonce, salt, tag, keytype, keystrength, fileContents);
        }
        public static void Save(string outputPath, byte[] nonce, byte[] salt, byte[] tag, G.KeyTypes keyType, G.KeyStrengths keyStrength, byte[] fileContents)
        {
            using var stream = File.Open(outputPath, FileMode.CreateNew, FileAccess.Write);
            var c = new A4CryptFile(nonce, salt, tag, (int)keyType, (int)keyStrength, fileContents);
            stream.Position = 0;
            stream.Write(G.ExpectedMagicSequence);
            stream.Write([G.ExpectedVersion]);
            stream.Write(c.Nonce);
            stream.Write(c.Salt);
            stream.Write(c.Tag);
            stream.Write([(byte)c.KeyType]);
            stream.Write([(byte)c.KeyStrength]);
            stream.Write(c.Contents);
        }

        private  static void SanityCheck(byte[] nonce, byte[] salt, byte[] tag)
        {
            if (nonce.Length != G.NonceSize || salt.Length != G.SaltSize || tag.Length != G.TagSize)

            {
                throw new FileInterfaceException($"Invalid file parameters");
            }
        }
        private static void HeaderCheck(FileStream stream)
        {
            MagicSequenceCheck(stream);
            CheckVersion(stream);
        }

        private static void MagicSequenceCheck(FileStream stream)
        {
            byte[] MagicSequence = new byte[4];
            stream.Position = G.MagicStart;
            stream.ReadExactly(MagicSequence);
            for (int i = 0; i < MagicSequence.Length; i++)
            {
                if (MagicSequence[i] != G.ExpectedMagicSequence[i])
                {
                    throw new FileInterfaceException("Invalid magic sequence");
                }
            }
        }

        private static void CheckVersion(FileStream stream)
        {
            stream.Position = G.VersionStart;
            int version = stream.ReadByte();
            if (version != G.ExpectedVersion)
            {
                throw new FileInterfaceException($"Invalid version {version}");
            }
        }
    }
    public class FileInterfaceException(string message) : Exception(message);
}
