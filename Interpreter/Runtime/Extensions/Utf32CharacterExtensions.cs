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
        public static string ToCharString(this Utf32Character value)
        {
            return char.ConvertFromUtf32(value.Value);
        }
    }
}
