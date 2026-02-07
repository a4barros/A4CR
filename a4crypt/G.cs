using System;
using System.Collections.Generic;
using System.Text;

namespace a4crypt
{
    static public class G
    {
        /* File layout:
         * [ magic ]    4 bytes  A4CR
         * [ version ]  1 byte   1
         * [ nonce ]   12 bytes
         * [ salt ]    16 bytes
         * [ tag ]     16 bytes
         * [ file contents ]
         */

        public const int MagicSize = 4;
        public const int VersionSize = 1;
        public const int NonceSize = 12;
        public const int SaltSize = 16;
        public const int TagSize = 16;

        public const int MagicStart = 0;
        public const int VersionStart = MagicStart + MagicSize;
        public const int NonceStart = VersionStart + VersionSize;
        public const int SaltStart = NonceStart + NonceSize;
        public const int TagStart = SaltStart + SaltSize;
        public const int FileContentsStart = TagStart + TagSize;

        public static readonly byte[] ExpectedMagicSequence = [(byte)'A', (byte)'4', (byte)'C', (byte)'R',];
        public const int ExpectedVersion = 1;
        public const int KeySize = 32;
        public const int Iterations = 600000;
    }
}