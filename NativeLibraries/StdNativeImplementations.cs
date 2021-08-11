﻿using Interpreter;
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
                return obj.ToString();
        }
    

        [ImplementationIdentifier("std.print")]
        public static readonly NativeFunctionStatementModel Print = new NativeFunctionStatementModel("obj")
        {
            NativeImplementation = args =>
            {
                if (args.Count is 0)
                    return null; // Do nothing

                IBinaryOperable obj = args[0];

                if (obj.OperableType == ObjectType.NullReference)
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

                IBinaryOperable obj = args[0];
                
                if (obj.OperableType == ObjectType.NullReference)
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
            NativeImplementation = () =>
            {
                string input = Console.ReadLine();
                return new StringWrapper(input);
            }
        };

        [ImplementationIdentifier("std.number")]
        public static readonly NativeFunctionStatementModel Number = new NativeFunctionStatementModel("s")
        {
            NativeImplementation = s =>
            {
                IBinaryOperable v = s[0];
                if (v.OperableType == ObjectType.String)
                {
                    return new ArbitraryBitIntegerWrapper(BigInteger.Parse((v as IBinaryOperable<string>).Value));
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
                IBinaryOperable value = args[0];

                if (value.OperableType == ObjectType.ArbitraryBitInteger)
                    return value;

                return new ArbitraryPrecisionDecimalWrapper(new BigDecimal((value as IBinaryOperable<BigInteger>).Value, 0));
            }
        };

        [ImplementationIdentifier("std.Convert.decimal.__to_int")]
        public static readonly NativeFunctionStatementModel ConvertDecimalToInt = new NativeFunctionStatementModel("value")
        {
            NativeImplementation = args =>
            {
                IBinaryOperable value = args[0];

                if (value.OperableType == ObjectType.ArbitraryBitInteger)
                    return value;

                return new ArbitraryBitIntegerWrapper((BigInteger)(value as IBinaryOperable<BigDecimal>).Value);
            }
        };

        [ImplementationIdentifier("std.__is_number")]
        public static readonly NativeFunctionStatementModel IsNumber = new NativeFunctionStatementModel("value")
        {
            NativeImplementation = args =>
            {
                IBinaryOperable value = args[0];
                return BooleanWrapper.FromBool(value.OperableType == ObjectType.ArbitraryBitInteger || value.OperableType == ObjectType.ArbitraryPrecisionDecimal);
            }
        };

        [ImplementationIdentifier("std.__is_integer")]
        public static readonly NativeFunctionStatementModel IsInteger = new NativeFunctionStatementModel("value")
        {
            NativeImplementation = args => BooleanWrapper.FromBool(args[0].OperableType == ObjectType.ArbitraryBitInteger)
        };

        [ImplementationIdentifier("std.__is_decimal")]
        public static readonly NativeFunctionStatementModel IsDecimal = new NativeFunctionStatementModel("value")
        {
            NativeImplementation = args => BooleanWrapper.FromBool(args[0].OperableType == ObjectType.ArbitraryPrecisionDecimal)
        };

        [ImplementationIdentifier("std.__is_string")]
        public static readonly NativeFunctionStatementModel IsString = new NativeFunctionStatementModel("value")
        {
            NativeImplementation = args => BooleanWrapper.FromBool(args[0].OperableType == ObjectType.String)
        };

        [ImplementationIdentifier("std.__is_bool")]
        public static readonly NativeFunctionStatementModel IsBool = new NativeFunctionStatementModel("value")
        {
            NativeImplementation = args => BooleanWrapper.FromBool(args[0].OperableType == ObjectType.Boolean)
        };

        [ImplementationIdentifier("std.__is_object")]
        public static readonly NativeFunctionStatementModel IsObject = new NativeFunctionStatementModel("value")
        {
            NativeImplementation = args => BooleanWrapper.FromBool(args[0].OperableType == ObjectType.Object)
        };

        [ImplementationIdentifier("std.__is_method")]
        public static readonly NativeFunctionStatementModel IsMethodGroup = new NativeFunctionStatementModel("value")
        {
            NativeImplementation = args =>
            {
                IBinaryOperable value = args[0];
                return BooleanWrapper.FromBool(value.OperableType == ObjectType.MethodData || value.OperableType == ObjectType.Method);
            }
        };

        [ImplementationIdentifier("std.delay")]
        public static readonly NativeFunctionStatementModel Delay = new NativeFunctionStatementModel("millis")
        {
            NativeImplementation = millis =>
            {
                Thread.Sleep((int)(millis[0] as IBinaryOperable<BigInteger>).Value);
                return null;
            }
        };

        [ImplementationIdentifier("std.systemMillis")]
        public static readonly NativeProviderStatementModel MillisecondsSinceSystemStart = new NativeProviderStatementModel
        {
            NativeImplementation = () => new ArbitraryBitIntegerWrapper(new BigInteger(Environment.TickCount64))
        };

        [ImplementationIdentifier("std.ticks")]
        public static readonly NativeProviderStatementModel TicksSinceUnixStart = new NativeProviderStatementModel
        {
            NativeImplementation = () => new ArbitraryBitIntegerWrapper(new BigInteger(DateTime.UtcNow.Ticks))
        };

        [ImplementationIdentifier("std.__array_allocate")]
        public static readonly NativeFunctionStatementModel ArrayAlloc = new NativeFunctionStatementModel("size")
        {
            NativeImplementation = size =>
            {
                IBinaryOperable indexNumber = size[0];
                int s = indexNumber.OperableType switch
                {
                    ObjectType.ArbitraryBitInteger => (int)(BigInteger)indexNumber.Value,
                    ObjectType.UnsignedByte => (byte)indexNumber.Value,
                    _ => throw new InvalidCastException($"Cannot cast type {indexNumber.Value.GetType()} to int")
                };
                IBinaryOperable[] array = Enumerable.Repeat(NullReferenceWrapper.Null, s).ToArray<IBinaryOperable>();
                return new ArrayWrapper(array);
            }
        };

        [ImplementationIdentifier("std.__array_length")]
        public static readonly NativeFunctionStatementModel ArrayLength = new NativeFunctionStatementModel("arrayRef")
        {
            NativeImplementation = arrayRef =>
            {
                IBinaryOperable[] arr = (arrayRef[0] as IBinaryOperable<IBinaryOperable[]>).Value;
                return new ArbitraryBitIntegerWrapper(new BigInteger(arr.Length));
            }
        };

        [ImplementationIdentifier("std.__array_get")]
        public static readonly NativeFunctionStatementModel ArrayGet = new NativeFunctionStatementModel("arrayRef", "index")
        {
            NativeImplementation = args =>
            {
                IBinaryOperable[] arr = (args[0] as IBinaryOperable<IBinaryOperable[]>).Value;
                IBinaryOperable indexNumber = args[1];
                int index = indexNumber.OperableType switch
                {
                    ObjectType.ArbitraryBitInteger => (int)(BigInteger)indexNumber.Value,
                    ObjectType.UnsignedByte => (byte)indexNumber.Value,
                    _ => throw new InvalidCastException($"Cannot cast type {indexNumber.Value.GetType()} to int")
                };
                return arr[index];
            }
        };

        [ImplementationIdentifier("std.__array_set")]
        public static readonly NativeFunctionStatementModel ArraySet = new NativeFunctionStatementModel("arrayRef", "index", "value")
        {
            NativeImplementation = args =>
            {
                IBinaryOperable[] arr = (args[0] as IBinaryOperable<IBinaryOperable[]>).Value;
                IBinaryOperable indexNumber = args[1];
                int index = indexNumber.OperableType switch
                {
                    ObjectType.ArbitraryBitInteger => (int)(BigInteger)indexNumber.Value,
                    ObjectType.UnsignedByte => (byte)indexNumber.Value,
                    _ => throw new InvalidCastException($"Cannot cast type {indexNumber.Value.GetType()} to int")
                };
                arr[index] = args[2];
                return null;
            }
        };

        [ImplementationIdentifier("std.String.__string_array")]
        public static readonly NativeFunctionStatementModel StringEnumerable = new NativeFunctionStatementModel("s")
        {
            NativeImplementation = args =>
            {
                string s = (string)args[0].Value;
                IBinaryOperable[] sCharArray = s.Select(c => new StringWrapper(c.ToString())).ToArray();
                return new ArrayWrapper(sCharArray);
            }
        };

        [ImplementationIdentifier("std.__dict_allocate")]
        public static readonly NativeProviderStatementModel DictionaryCreate = new NativeProviderStatementModel
        {
            NativeImplementation = () => new DictionaryWrapper(new Dictionary<IBinaryOperable, IBinaryOperable>())
        };

        [ImplementationIdentifier("std.__dict_set")]
        public static readonly NativeFunctionStatementModel DictionarySet = new NativeFunctionStatementModel("dictRef", "key", "value")
        {
            NativeImplementation = args =>
            {
                var dictRef = (IDictionary<IBinaryOperable, IBinaryOperable>)args[0].Value;
                IBinaryOperable key = args[1];
                IBinaryOperable value = args[2];
                dictRef[key] = value;
                return NullReferenceWrapper.Null;
            }
        };

        [ImplementationIdentifier("std.__dict_get")]
        public static readonly NativeFunctionStatementModel DictionaryGet = new NativeFunctionStatementModel("dictRef", "key")
        {
            NativeImplementation = args =>
            {
                var dictRef = (IDictionary<IBinaryOperable, IBinaryOperable>)args[0].Value;
                IBinaryOperable key = args[1];
                return dictRef[key];
            }
        };

        [ImplementationIdentifier("std.__dict_contains")]
        public static readonly NativeFunctionStatementModel DictionaryContains = new NativeFunctionStatementModel("dictRef", "key")
        {
            NativeImplementation = args =>
            {
                var dictRef = (IDictionary<IBinaryOperable, IBinaryOperable>)args[0].Value;
                IBinaryOperable key = args[1];
                return BooleanWrapper.FromBool(dictRef.ContainsKey(key));
            }
        };

        [ImplementationIdentifier("std.__dict_remove")]
        public static readonly NativeFunctionStatementModel DictionaryRemove = new NativeFunctionStatementModel("dictRef", "key")
        {
            NativeImplementation = args =>
            {
                var dictRef = (IDictionary<IBinaryOperable, IBinaryOperable>)args[0].Value;
                IBinaryOperable key = args[1];
                return BooleanWrapper.FromBool(dictRef.Remove(key));
            }
        };

        [ImplementationIdentifier("std.__dict_length")]
        public static readonly NativeFunctionStatementModel DictionaryLength = new NativeFunctionStatementModel("dictRef")
        {
            NativeImplementation = args =>
            {
                var dictRef = (IDictionary<IBinaryOperable, IBinaryOperable>)args[0].Value;
                return new ArbitraryBitIntegerWrapper(new BigInteger(dictRef.Count));
            }
        };

        [ImplementationIdentifier("std.__dict_get_key_array")]
        public static readonly NativeFunctionStatementModel DictionaryGetKeyArray = new NativeFunctionStatementModel("dictRef")
        {
            NativeImplementation = args =>
            {
                var dictRef = (IDictionary<IBinaryOperable, IBinaryOperable>)args[0].Value;
                return new ArrayWrapper(dictRef.Keys.ToArray());
            }
        };

        [ImplementationIdentifier("std.__dict_get_value_array")]
        public static readonly NativeFunctionStatementModel DictionaryGetValueArray = new NativeFunctionStatementModel("dictRef")
        {
            NativeImplementation = args =>
            {
                var dictRef = (IDictionary<IBinaryOperable, IBinaryOperable>)args[0].Value;
                return new ArrayWrapper(dictRef.Values.ToArray());
            }
        };
    }
}
