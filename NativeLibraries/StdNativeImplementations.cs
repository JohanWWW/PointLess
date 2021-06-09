using Interpreter;
using Interpreter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NativeLibraries
{
    public class StdNativeImplementations : NativeImplementationBase
    {
        [ImplementationIdentifier("std.print")]
        public static readonly NativeFunctionStatementModel Print = new NativeFunctionStatementModel("obj")
        {
            NativeImplementation = obj =>
            {
                string s = obj[0]?.ToString() ?? "null";
                s = s.Replace("\\n", "\n");
                Console.Write(s);
                return null;
            }
        };

        [ImplementationIdentifier("std.println")]
        public static readonly NativeFunctionStatementModel Println = new NativeFunctionStatementModel("obj")
        {
            NativeImplementation = obj =>
            {
                string s = obj[0]?.ToString() ?? "null";
                s = s.Replace("\\n", "\n");
                Console.WriteLine(s);
                return null;
            }
        };

        [ImplementationIdentifier("std.readln")]
        public static readonly NativeProviderStatementModel Readln = new NativeProviderStatementModel
        {
            NativeImplementation = () => Console.ReadLine()
        };

        [ImplementationIdentifier("std.number")]
        public static readonly NativeFunctionStatementModel Number = new NativeFunctionStatementModel("s")
        {
            NativeImplementation = s =>
            {
                dynamic v = s[0];
                if (v is string)
                {
                    return BigInteger.Parse(v as string);
                }
                else
                {
                    throw new NotSupportedException("Cannot cast provided type");
                }
            }
        };

        [ImplementationIdentifier("std.delay")]
        public static readonly NativeFunctionStatementModel Delay = new NativeFunctionStatementModel("millis")
        {
            NativeImplementation = millis =>
            {
                Thread.Sleep((int)millis[0]);
                return null;
            }
        };

        [ImplementationIdentifier("std.Array.__array_allocate")]
        public static readonly NativeFunctionStatementModel ArrayAlloc = new NativeFunctionStatementModel("size")
        {
            NativeImplementation = size =>
            {
                int s = (int)size[0];
                dynamic[] array = new dynamic[s];
                return array;
            }
        };

        [ImplementationIdentifier("std.Array.__array_length")]
        public static readonly NativeFunctionStatementModel ArrayLength = new NativeFunctionStatementModel("arrayRef")
        {
            NativeImplementation = arrayRef =>
            {
                dynamic[] arr = (dynamic[])arrayRef[0];
                return new BigInteger(arr.Length);
            }
        };

        [ImplementationIdentifier("std.Array.__array_get")]
        public static readonly NativeFunctionStatementModel ArrayGet = new NativeFunctionStatementModel("arrayRef", "index")
        {
            NativeImplementation = args =>
            {
                dynamic[] arr = (dynamic[])args[0];
                return arr[(int)args[1]];
            }
        };

        [ImplementationIdentifier("std.Array.__array_set")]
        public static readonly NativeFunctionStatementModel ArraySet = new NativeFunctionStatementModel("arrayRef", "index", "value")
        {
            NativeImplementation = args =>
            {
                dynamic[] arr = (dynamic[])args[0];
                int index = (int)args[1];
                arr[index] = args[2];
                return null;
            }
        };

        [ImplementationIdentifier("std.String.__string_array")]
        public static readonly NativeFunctionStatementModel StringEnumerable = new NativeFunctionStatementModel("s")
        {
            NativeImplementation = args =>
            {
                string s = (string)args[0];
                dynamic[] sCharArray = s.Select(c => c.ToString()).ToArray();
                return sCharArray;
            }
        };
    }
}
