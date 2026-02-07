using System;

namespace a4crypt
{
    static public class G
    {
        /* File layout:
         * [ magic ]        4 bytes  A4CR
         * [ version ]      1 byte   1
         * [ nonce ]       12 bytes
         * [ salt ]        16 bytes
         * [ tag ]         16 bytes
         * [ key type ]     1 byte   (0 = PBKDF2, 1 = Argon2id)
         * [ key strength ] 1 byte   0 to 3 (higher is safer)
         * [ file contents ]
         */

        public const int MagicSize = 4;
        public const int VersionSize = 1;
        public const int NonceSize = 12;
        public const int SaltSize = 16;
        public const int TagSize = 16;
        public const int KeyTypeSize = 1;
        public const int KeyStrengthSize = 1;

        public const int MagicStart = 0;
        public const int VersionStart = MagicStart + MagicSize;
        public const int NonceStart = VersionStart + VersionSize;
        public const int SaltStart = NonceStart + NonceSize;
        public const int TagStart = SaltStart + SaltSize;
        public const int KeyTypeStart = TagStart + TagSize;
        public const int KeyStrengthStart = KeyTypeStart + KeyTypeSize;
        public const int FileContentsStart = KeyStrengthStart + KeyStrengthSize;

        public static readonly byte[] ExpectedMagicSequence = new byte[] { (byte)'A', (byte)'4', (byte)'C', (byte)'R' };
        public const int ExpectedVersion = 1;
        public const int KeySize = 32;

        public enum KeyTypes
        {
            PBKDF2 = 0,
            Argon2id = 1,
        }

        public enum KeyStrengths
        {
            Low = 0,
            Medium = 1,
            High = 2,
            Ultra = 3,
        }

        public static readonly List<Pbkdf2Strength> Pbkdf2Strengths =
        [
            new Pbkdf2Strength(100000),
            new Pbkdf2Strength(300000),
            new Pbkdf2Strength(600000),
            new Pbkdf2Strength(1200000),
        ];

        public static readonly List<Argon2idStrength> Argon2idStrengths =
        [
            new Argon2idStrength(2, 64, 1),
            new Argon2idStrength(3, 128, 2),
            new Argon2idStrength(4, 256, 3),
            new Argon2idStrength(6, 512, 4),
        ];

        public record Argon2idStrength (
            int Time,
            int MemoryMB,
            int Parallelism
        );
        public record Pbkdf2Strength (
            int Iterations
        );
    }
}