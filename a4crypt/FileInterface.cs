using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace a4crypt
{
    internal class FileInterface
    {
        /* File layout:
         * [ magic ]    4 bytes  A4CR
         * [ version ]  1 byte   1
         * [ nonce ]   12 bytes
         * [ salt ]    16 bytes
         * [ tag ]     16 bytes
         * [ file contents ]
         */

        private const int MagicSize = 4;
        private const int VersionSize = 1;
        private const int NonceSize = 12;
        private const int SaltSize = 16;
        private const int TagSize = 16;

        private const int MagicStart = 0;
        private const int VersionStart = MagicStart + MagicSize;
        private const int NonceStart = VersionStart + VersionSize;
        private const int SaltStart = NonceStart + NonceSize;
        private const int TagStart = SaltStart + SaltSize;
        private const int FileContentsStart = TagStart + TagSize;

        public static readonly byte[] ExpectedMagicSequence = [(byte)'A', (byte)'4', (byte)'C', (byte)'R',];
        private const int ExpectedVersion = 1;

        private readonly byte[] Nonce;
        private readonly byte[] Salt;
        public byte[] Tag { get; }
        public byte[] Contents { get; }
        public static FileInterface Open(string inputPath)
        {
            var stream = File.Open(inputPath, FileMode.Open, FileAccess.Read);
            HeaderCheck(stream);

            stream.Position = NonceStart;
            var nonce = new byte[NonceSize];
            stream.ReadExactly(nonce);

            stream.Position = SaltStart;
            var salt = new byte[SaltSize];
            stream.ReadExactly(salt);

            stream.Position = TagStart;
            var tag = new byte[TagSize];
            stream.ReadExactly(tag);

            stream.Position = FileContentsStart;
            long fileLength = stream.Length - FileContentsStart;
            var fileContents = new byte[fileLength];
            stream.ReadExactly(fileContents);
            stream.Close();

            return new FileInterface(nonce, salt, tag, fileContents);
        }
        public void Save(string outputPath, byte[] nonce, byte[] salt, byte[] tag, byte[] fileContents)
        {
            SaveSanityCheck(nonce, salt, tag);
            var stream = File.Open(outputPath, FileMode.CreateNew, FileAccess.Write);
            stream.Position = 0;
            stream.Write(ExpectedMagicSequence);
            stream.Write([ExpectedVersion]);
            stream.Write(Nonce);
            stream.Write(Salt);
            stream.Write(Tag);
            stream.Write(fileContents);
            stream.Close();
        }

        private void SaveSanityCheck(byte[] nonce, byte[] salt, byte[] tag)
        {
            if (nonce.Length != NonceSize || salt.Length != Salt.Length || tag.Length != TagSize)
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
            stream.Position = MagicStart;
            stream.ReadExactly(MagicSequence);
            for (int i = 0; i < MagicSequence.Length; i++)
            {
                if (MagicSequence[i] != ExpectedMagicSequence[i])
                {
                    throw new FileInterfaceException("Invalid magic sequence");
                }
            }
        }

        private static void CheckVersion(FileStream stream)
        {
            stream.Position = VersionStart;
            int version = stream.ReadByte();
            if (version != ExpectedVersion)
            {
                throw new FileInterfaceException($"Invalid version {version}");
            }
        }

        private FileInterface(byte[] nonce, byte[] salt, byte[] tag, byte[] contents) 
        { 
            Nonce = nonce;
            Salt = salt;
            Tag = tag;
            Contents = contents;
        }
    }
    class FileInterfaceException(string message) : Exception(message);
}
