using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Helpers
{
    internal static class NumberParserHelper
    {
        private const int BINARY_BASE = 2;
        private const int HEX_BASE = 16;

        private static readonly IReadOnlyDictionary<char, byte> HEX_VALUES = new Dictionary<char, byte>
        {
            ['0'] = 0,
            ['1'] = 1,
            ['2'] = 2,
            ['3'] = 3,
            ['4'] = 4,
            ['5'] = 5,
            ['6'] = 6,
            ['7'] = 7,
            ['8'] = 8,
            ['9'] = 9,
            ['A'] = 10, ['a'] = 10,
            ['B'] = 11, ['b'] = 11,
            ['C'] = 12, ['c'] = 12,
            ['D'] = 13, ['d'] = 13,
            ['E'] = 14, ['e'] = 14,
            ['F'] = 15, ['f'] = 15
        };

        private static bool[] GetBits(ReadOnlySpan<char> bin)
        {
            bool[] bits = new bool[bin.Length];
            int i = 0;
            var enumerator = bin.GetEnumerator();
            while (enumerator.MoveNext())
            {
                char c = enumerator.Current;
                bits[i] = c is '1' || (c is '0' ? false : throw new FormatException("Invalid character: " + c));
                i++;
            }
            return bits;
        }

        private static int Pow(this int x, int e)
        {
            int p = 1;
            for (int i = e; i > 0; p *= x, i--)
            {
            }
            return p;
        }

        private static BigInteger Pow(this BigInteger x, int e) => BigInteger.Pow(x, e);

        public static byte BinaryToUByte(ReadOnlySpan<char> bin)
        {
            bool[] bits = GetBits(bin);

            if (bits.Length > 8)
                throw new FormatException("Input string had more than 8 characters");

            byte value = 0;
            int i = 0;
            int exponent = bits.Length - 1;
            while (i < bits.Length)
            {
                if (bits[i])
                    value += (byte)BINARY_BASE.Pow(exponent);

                i++;
                exponent--;
            }

            return value;
        }

        public static BigInteger BinaryToBigInt(ReadOnlySpan<char> bin)
        {
            bool[] bits = GetBits(bin);

            BigInteger binaryBase = BINARY_BASE;

            BigInteger value = 0;
            int i = 0;
            int exponent = bits.Length - 1;
            while (i < bits.Length)
            {
                if (bits[i])
                    value += binaryBase.Pow(exponent);

                i++;
                exponent--;
            }

            return value;
        }

        public static byte HexToUByte(ReadOnlySpan<char> hex)
        {
            if (hex.Length > 2)
                throw new FormatException("Input string had more than 2 characters");

            byte value = 0;
            int exponent = hex.Length - 1;
            var enumerator = hex.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (!HEX_VALUES.TryGetValue(enumerator.Current, out byte hexVal))
                    throw new FormatException("Invalid character: " + enumerator.Current);

                value += (byte)(hexVal * HEX_BASE.Pow(exponent));

                exponent--;
            }
            return value;
        }

        public static BigInteger HexToBigInt(ReadOnlySpan<char> hex)
        {
            BigInteger hexBase = HEX_BASE;
            BigInteger value = 0;
            int exponent = hex.Length - 1;
            var enumerator = hex.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (!HEX_VALUES.TryGetValue(enumerator.Current, out byte hexVal))
                    throw new FormatException("Invalid character: " + enumerator.Current);

                value += hexVal * hexBase.Pow(exponent);

                exponent--;
            }
            return value;
        }

        public static int HexToInt(ReadOnlySpan<char> hex)
        {
            int value = 0;
            int exponent = hex.Length - 1;
            var enumerator = hex.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (!HEX_VALUES.TryGetValue(enumerator.Current, out byte hexVal))
                    throw new FormatException("Invalid character: " + enumerator.Current);

                value += hexVal * HEX_BASE.Pow(exponent);

                exponent--;
            }
            return value;
        }
    }
}
