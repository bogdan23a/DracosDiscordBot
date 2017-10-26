using System;

namespace Couchbase.Lite
{
    internal static class CharUtils
    {
        public static int Digit(char character, int radix) {
            if (radix != 16)
                throw new ArgumentException("Only hex/base16 is supported.", "radix");
            return HexToInt(character);
        }

        // COPY: Copied from libcore.net.UriCodec
        /// <summary>
        /// Like
        /// <see cref="char.Digit(char, int)">char.Digit(char, int)</see>
        /// , but without support for non-ASCII
        /// characters.
        /// </summary>
        internal static int HexToInt(char c)
        {
            if ('0' <= c && c <= '9')
            {
                return c - '0';
            }
            else
            {
                if ('a' <= c && c <= 'f')
                {
                    return 10 + (c - 'a');
                }
                else
                {
                    if ('A' <= c && c <= 'F')
                    {
                        return 10 + (c - 'A');
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
        }
    }
}

