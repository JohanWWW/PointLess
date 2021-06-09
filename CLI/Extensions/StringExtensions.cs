using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroPointCLI.Extensions
{
    internal static class StringExtensions
    {
        public static string FirstToLower(this string source)
        {
            if (source.Length is 0 || char.IsLower(source[0]))
                return source;

            char[] array = source.ToCharArray();
            array[0] = char.ToLower(array[0]);

            return new string(array);
        }

        public static string FirstToUpper(this string source)
        {
            if (source.Length is 0 || char.IsUpper(source[0]))
                return source;

            char[] array = source.ToCharArray();
            array[0] = char.ToUpper(array[0]);

            return new string(array);
        }
    }
}
