using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Types
{
    public class Utf32String : IEnumerable<Utf32Character>, IEquatable<Utf32String>
    {
        private readonly Utf32Character[] _value;

        public int Length => _value.Length;

        /// <summary>
        /// Returns the amount of bytes that are allocated for this string
        /// </summary>
        public int SizeOf => Utf32Character.SIZE * Length;

        public Utf32String(ReadOnlySpan<Utf32Character> value)
        {
            _value = value.ToArray();
        }

        public Utf32String(ReadOnlySpan<char> value)
        {
            Utf32Character[] utf32Characters = new Utf32Character[value.Length];
            for (int i = 0; i < utf32Characters.Length; i++)
                utf32Characters[i] = value[i];
            _value = utf32Characters;
        }

        private Utf32String(Utf32Character[] value)
        {
            _value = value;
        }

        public Utf32Character this[int index] => _value[index];

        public byte[] GetBytes() => 
            _value.SelectMany(c => c.GetBytes()).ToArray();

        public bool Equals(Utf32String other) => 
            Equals((object)other);

        public override bool Equals(object obj) => 
            GetHashCode() == obj.GetHashCode();

        public IEnumerator<Utf32Character> GetEnumerator() => _value.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override unsafe int GetHashCode()
        {
            fixed (Utf32Character* src = _value)
            {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;

                Utf32Character* cPtr = src;
                int length = Length;
                while (length > 2)
                {
                    hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ cPtr[0].Value;
                    hash2 = ((hash2 << 5) + hash2 + (hash2 >> 27)) ^ cPtr[1].Value;
                    cPtr += 2;
                    length -= 4;
                }

                if (length > 0)
                    hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ cPtr[0].Value;

                return hash1 + (hash2 * 1566083941);
            }
        }

        public override string ToString() => Encoding.UTF32.GetString(GetBytes());


        #region Operator Overloads

        public static implicit operator Utf32String(string value) => new(value);

        public static bool operator ==(Utf32String a, Utf32String b) => a.Equals(b);
        public static bool operator !=(Utf32String a, Utf32String b) => !a.Equals(b);
        public static Utf32String operator +(Utf32String a, Utf32String b)
        {
            Utf32Character[] utf32Characters = new Utf32Character[a.Length + b.Length];
            int i = 0;
            for (int ii = 0; ii < a.Length; ii++, i++)
                utf32Characters[i] = a[ii];
            for (int ii = 0; ii < b.Length; ii++, i++)
                utf32Characters[i] = b[ii];
            return new Utf32String(utf32Characters);
        }

#endregion
    }
}
