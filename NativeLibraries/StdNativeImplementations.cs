using Interpreter;
using Interpreter.Environment;
using Interpreter.Models;
using Interpreter.Models.Delegates;
using Interpreter.Runtime;
using Singulink.Numerics;
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
        private static string FormatPrintText(object obj)
        {
            if (obj is RuntimeObject)
            {
                var rtObject = obj as RuntimeObject;

                if (rtObject.TryGetMember(new RuntimeObject.GetterBinder("toString"), out object objectMember)
                    && objectMember is MethodData objectToStringFunction
                    && objectToStringFunction.GetOverload(0).ParameterCount is 0)
                    return (string)objectToStringFunction.GetOverload(0).GetProvider().Invoke();
                else
                    return rtObject.ToString();
            }
            else
                return obj.ToString();
        }
    

        [ImplementationIdentifier("std.print")]
        public static readonly NativeFunctionStatementModel Print = new NativeFunctionStatementModel("obj")
        {
            NativeImplementation = args =>
            {
                if (args.Count is 0)
                    return null; // Do nothing

                dynamic obj = args[0];

                if (obj is null)
                {
                    Console.Write("null");
                    return null;
                }

                Console.Write(FormatPrintText(obj));

                return null;
            }
        };

        [ImplementationIdentifier("std.println")]
        public static readonly NativeFunctionStatementModel Println = new NativeFunctionStatementModel("obj")
        {
            NativeImplementation = args =>
            {
                if (args.Count is 0)
                    Console.WriteLine();

                dynamic obj = args[0];
                
                if (obj is null)
                {
                    Console.WriteLine("null");
                    return null;
                }

                Console.WriteLine(FormatPrintText(obj));

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

        [ImplementationIdentifier("std.Convert.int.__to_decimal")]
        public static readonly NativeFunctionStatementModel ConvertIntToDecimal = new NativeFunctionStatementModel("value")
        {
            NativeImplementation = args =>
            {
                dynamic value = args[0];

                if (value is BigInteger)
                    return value;

                return new BigDecimal(value, 0);
            }
        };

        [ImplementationIdentifier("std.Convert.decimal.__to_int")]
        public static readonly NativeFunctionStatementModel ConvertDecimalToInt = new NativeFunctionStatementModel("value")
        {
            NativeImplementation = args =>
            {
                dynamic value = args[0];

                if (value is BigInteger)
                    return value;

                return (BigInteger)value;
            }
        };

        [ImplementationIdentifier("std.__is_number")]
        public static readonly NativeFunctionStatementModel IsNumber = new NativeFunctionStatementModel("value")
        {
            NativeImplementation = args =>
            {
                dynamic value = args[0];
                return value is BigInteger || value is BigDecimal;
            }
        };

        [ImplementationIdentifier("std.__is_integer")]
        public static readonly NativeFunctionStatementModel IsInteger = new NativeFunctionStatementModel("value")
        {
            NativeImplementation = args => args[0] is BigInteger
        };

        [ImplementationIdentifier("std.__is_decimal")]
        public static readonly NativeFunctionStatementModel IsDecimal = new NativeFunctionStatementModel("value")
        {
            NativeImplementation = args => args[0] is BigDecimal
        };

        [ImplementationIdentifier("std.__is_string")]
        public static readonly NativeFunctionStatementModel IsString = new NativeFunctionStatementModel("value")
        {
            NativeImplementation = args => args[0] is string
        };

        [ImplementationIdentifier("std.__is_bool")]
        public static readonly NativeFunctionStatementModel IsBool = new NativeFunctionStatementModel("value")
        {
            NativeImplementation = args => args[0] is bool
        };

        [ImplementationIdentifier("std.__is_object")]
        public static readonly NativeFunctionStatementModel IsObject = new NativeFunctionStatementModel("value")
        {
            NativeImplementation = args => args[0] is RuntimeObject
        };

        [ImplementationIdentifier("std.__is_method")]
        public static readonly NativeFunctionStatementModel IsMethodGroup = new NativeFunctionStatementModel("value")
        {
            NativeImplementation = args =>
            {
                dynamic value = args[0];
                return value is MethodData || value is Method;
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

        [ImplementationIdentifier("std.systemMillis")]
        public static readonly NativeProviderStatementModel MillisecondsSinceSystemStart = new NativeProviderStatementModel
        {
            NativeImplementation = () => new BigInteger(Environment.TickCount64)
        };

        [ImplementationIdentifier("std.ticks")]
        public static readonly NativeProviderStatementModel TicksSinceUnixStart = new NativeProviderStatementModel
        {
            NativeImplementation = () => new BigInteger(DateTime.UtcNow.Ticks)
        };

        [ImplementationIdentifier("std.__array_allocate")]
        public static readonly NativeFunctionStatementModel ArrayAlloc = new NativeFunctionStatementModel("size")
        {
            NativeImplementation = size =>
            {
                int s = (int)size[0];
                dynamic[] array = new dynamic[s];
                return array;
            }
        };

        [ImplementationIdentifier("std.__array_length")]
        public static readonly NativeFunctionStatementModel ArrayLength = new NativeFunctionStatementModel("arrayRef")
        {
            NativeImplementation = arrayRef =>
            {
                dynamic[] arr = (dynamic[])arrayRef[0];
                return new BigInteger(arr.Length);
            }
        };

        [ImplementationIdentifier("std.__array_get")]
        public static readonly NativeFunctionStatementModel ArrayGet = new NativeFunctionStatementModel("arrayRef", "index")
        {
            NativeImplementation = args =>
            {
                dynamic[] arr = (dynamic[])args[0];
                return arr[(int)args[1]];
            }
        };

        [ImplementationIdentifier("std.__array_set")]
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

        [ImplementationIdentifier("std.__dict_allocate")]
        public static readonly NativeProviderStatementModel DictionaryCreate = new NativeProviderStatementModel
        {
            NativeImplementation = () => new Dictionary<dynamic, dynamic>()
        };

        [ImplementationIdentifier("std.__dict_set")]
        public static readonly NativeFunctionStatementModel DictionarySet = new NativeFunctionStatementModel("dictRef", "key", "value")
        {
            NativeImplementation = args =>
            {
                var dictRef = (Dictionary<dynamic, dynamic>)args[0];
                dynamic key = args[1];
                dynamic value = args[2];
                dictRef[key] = value;
                return null;
            }
        };

        [ImplementationIdentifier("std.__dict_get")]
        public static readonly NativeFunctionStatementModel DictionaryGet = new NativeFunctionStatementModel("dictRef", "key")
        {
            NativeImplementation = args =>
            {
                var dictRef = (Dictionary<dynamic, dynamic>)args[0];
                dynamic key = args[1];
                return dictRef[key];
            }
        };

        [ImplementationIdentifier("std.__dict_contains")]
        public static readonly NativeFunctionStatementModel DictionaryContains = new NativeFunctionStatementModel("dictRef", "key")
        {
            NativeImplementation = args =>
            {
                var dictRef = (Dictionary<dynamic, dynamic>)args[0];
                dynamic key = args[1];
                return dictRef.ContainsKey(key);
            }
        };

        [ImplementationIdentifier("std.__dict_remove")]
        public static readonly NativeFunctionStatementModel DictionaryRemove = new NativeFunctionStatementModel("dictRef", "key")
        {
            NativeImplementation = args =>
            {
                var dictRef = (Dictionary<dynamic, dynamic>)args[0];
                dynamic key = args[1];
                return dictRef.Remove(key);
            }
        };

        [ImplementationIdentifier("std.__dict_length")]
        public static readonly NativeFunctionStatementModel DictionaryLength = new NativeFunctionStatementModel("dictRef")
        {
            NativeImplementation = args =>
            {
                var dictRef = (Dictionary<dynamic, dynamic>)args[0];
                return new BigInteger(dictRef.Count);
            }
        };

        [ImplementationIdentifier("std.__dict_get_key_array")]
        public static readonly NativeFunctionStatementModel DictionaryGetKeyArray = new NativeFunctionStatementModel("dictRef")
        {
            NativeImplementation = args =>
            {
                var dictRef = (Dictionary<dynamic, dynamic>)args[0];
                return dictRef.Keys.ToArray();
            }
        };

        [ImplementationIdentifier("std.__dict_get_value_array")]
        public static readonly NativeFunctionStatementModel DictionaryGetValueArray = new NativeFunctionStatementModel("dictRef")
        {
            NativeImplementation = args =>
            {
                var dictRef = (Dictionary<dynamic, dynamic>)args[0];
                return dictRef.Values.ToArray();
            }
        };
    }
}
