using Interpreter.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Runtime.Extensions
{
    public static class Utf32CharacterExtensions
    {
        private const int BYTE_MASK = 0xFF;

        public static string ToCharString(this Utf32Character value)
        {
            return char.ConvertFromUtf32(value.Value);
        }

        public static IOperable ToFrameworkBytes(this Utf32Character value)
        {
            return new ArrayObjectOperable(new IOperable[4] 
            {
                (ByteOperable)(value.Value & BYTE_MASK),
                (ByteOperable)((value.Value >> 8) & BYTE_MASK),
                (ByteOperable)((value.Value >> 16) & BYTE_MASK),
                (ByteOperable)(value.Value >> 21)
            });
        }
    }
}
