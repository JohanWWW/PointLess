using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Types
{
    public struct Utf32Character : IEquatable<Utf32Character>
    {
        private const int BYTE_MASK = 0xFF;

        /// <summary>
        /// The size of a utf-32 character in bytes
        /// </summary>
        public const int SIZE = 4;

        /// <summary>
        /// The integer value of the character
        /// </summary>
        public readonly int Value;

        public Utf32Character(int value)
        {
            Value = value;
        }

        public Utf32Character(ReadOnlySpan<byte> bytes)
        {
            int c = bytes[0];
            c |= bytes[1] << 8;
            c |= bytes[2] << 16;
            c |= bytes[3] << 21;
            Value = c;
        }

        public byte[] GetBytes() => new byte[SIZE]
        {
            (byte)(Value & BYTE_MASK),
            (byte)((Value >> 8) & BYTE_MASK),
            (byte)((Value >> 16) & BYTE_MASK),
            (byte)(Value >> 21)
        };

        public bool Equals(Utf32Character other) => Value == other.Value;

        public override bool Equals(object obj) => Equals((Utf32Character)obj);

        public override int GetHashCode() => HashCode.Combine(Value);

        public override string ToString() => char.ConvertFromUtf32(Value);


        #region Operator Overloads

        public static implicit operator Utf32Character(int value) => new(value);

        public static bool operator ==(Utf32Character a, Utf32Character b) => a.Equals(b);
        public static bool operator !=(Utf32Character a, Utf32Character b) => !a.Equals(b);

        #endregion
    }
}
