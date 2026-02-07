using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace a4crypt
{
    internal class CryptFileInterface
    {
        public readonly byte[] Nonce;
        public readonly byte[] Salt;
        public byte[] Tag { get; }
        public byte[] Contents { get; }
        private CryptFileInterface(byte[] nonce, byte[] salt, byte[] tag, byte[] contents)
        {
            Nonce = nonce;
            Salt = salt;
            Tag = tag;
            Contents = contents;
        }
        public static CryptFileInterface Open(string inputPath)
        {
            var stream = File.Open(inputPath, FileMode.Open, FileAccess.Read);
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

            stream.Position = G.FileContentsStart;
            long fileLength = stream.Length - G.FileContentsStart;
            var fileContents = new byte[fileLength];
            stream.ReadExactly(fileContents);
            stream.Close();

            return new CryptFileInterface(nonce, salt, tag, fileContents);
        }
        public static void Save(string outputPath, byte[] nonce, byte[] salt, byte[] tag, byte[] fileContents)
        {
            SaveSanityCheck(nonce, salt, tag);
            var stream = File.Open(outputPath, FileMode.CreateNew, FileAccess.Write);
            stream.Position = 0;
            stream.Write(G.ExpectedMagicSequence);
            stream.Write([G.ExpectedVersion]);
            stream.Write(nonce);
            stream.Write(salt);
            stream.Write(tag);
            stream.Write(fileContents);
            stream.Close();
        }

        private  static void SaveSanityCheck(byte[] nonce, byte[] salt, byte[] tag)
        {
            if (nonce.Length != G.NonceSize || salt.Length != salt.Length || tag.Length != G.TagSize)
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
    class FileInterfaceException(string message) : Exception(message);
}
