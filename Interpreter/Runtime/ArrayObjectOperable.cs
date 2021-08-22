using Interpreter.Environment;
using Interpreter.Models.Delegates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Runtime
{
    public class ArrayObjectOperable : OperableBase<RuntimeObject>
    {
        #region Private constants
        private const string METHOD_NAME_LENGTH                 = "length";
        private const string METHOD_NAME_GET                    = "get";
        private const string METHOD_NAME_SET                    = "set";
        private const string METHOD_NAME_ENUMERATOR             = "enumerator";
        private const string METHOD_NAME_RANGE                  = "range";
        private const string METHOD_NAME_TO_STRING              = "toString";
        private const string METHOD_NAME_INDEXER_GET            = "__indexer_get__";
        private const string METHOD_NAME_INDEXER_SET            = "__indexer_set__";
        private const string METHOD_NAME_OPERATOR_EQUALS        = "__operator_equals__";
        private const string METHOD_NAME_OPERATOR_NOT_EQUALS    = "__operator_not_equals__";
        private const string METHOD_NAME_TO_OBJECT              = "toObject";
        #endregion

        private readonly IOperable[] _array;

        public ArrayObjectOperable() : this(Array.Empty<IOperable>())
        {
        }

        public ArrayObjectOperable(IOperable[] array) : base(GenerateRuntimeObject(array), ObjectType.ArrayObject)
        {
            _array = array;
        }

        private static RuntimeObject GenerateRuntimeObject(IOperable[] array)
        {
            RuntimeObject obj = new();

            var indexGet = GenerateGetMethod(array);
            var indexSet = GenerateSetMethod(array);

            obj[METHOD_NAME_LENGTH] = GenerateLengthMethod(array);
            obj[METHOD_NAME_GET] = indexGet;
            obj[METHOD_NAME_SET] = indexSet;
            obj[METHOD_NAME_ENUMERATOR] = GenerateEnumeratorMethod(array);
            obj[METHOD_NAME_RANGE] = GenerateRangeMethod(array);
            obj[METHOD_NAME_TO_STRING] = GenerateToStringMethod(array);
            obj[METHOD_NAME_INDEXER_GET] = indexGet;
            obj[METHOD_NAME_INDEXER_SET] = indexSet;
            obj[METHOD_NAME_TO_OBJECT] = GenerateToObjectMethod(obj);

            return obj;
        }

        #region Method constructor methods

        private static MethodDataOperable GenerateLengthMethod(IOperable[] array)
        {
            ProviderMethod length = () => (BigIntOperable)array.Length;
            Method lengthMethod = new(0, length, MethodType.Provider);
            return new MethodData(lengthMethod);
        }

        private static MethodDataOperable GenerateGetMethod(IOperable[] array)
        {
            FunctionMethod get = args =>
            {
                IOperable tmp = args[0];
                switch (tmp.OperableType)
                {
                    case ObjectType.ArbitraryBitInteger:
                        {
                            BigInteger tmp2 = (tmp as IOperable<BigInteger>).Value;
                            if (tmp2 < 0 || tmp2 > ulong.MaxValue)
                                throw new OperableException("Argument out of range");
                            if (tmp2 >= array.Length)
                                throw new OperableException("Index out of range");
                            ulong index = (ulong)tmp2;
                            return array[index];
                        }
                    case ObjectType.UnsignedByte:
                        {
                            byte index = (tmp as IOperable<byte>).Value;
                            if (index >= array.Length)
                                throw new OperableException("Index out of range");
                            return array[index];
                        }
                    default:
                        throw new OperableException($"An argument of illegal type '{tmp.OperableType}' was provided to the method");
                }
            };
            Method getMethod = new(1, get, MethodType.Function);
            return new MethodData(getMethod);
        }

        private static MethodDataOperable GenerateSetMethod(IOperable[] array)
        {
            ConsumerMethod set = args =>
            {
                IOperable indexArg = args[0];
                IOperable valueArg = args[1];

                switch (indexArg.OperableType)
                {
                    case ObjectType.ArbitraryBitInteger:
                        {
                            BigInteger tmp = (indexArg as IOperable<BigInteger>).Value;
                            if (tmp < 0 || tmp > ulong.MaxValue)
                                throw new OperableException("Argument out of range");
                            if (tmp >= array.Length)
                                throw new OperableException("Index out of range");
                            ulong index = (ulong)tmp;
                            array[index] = valueArg;
                            break;
                        }
                    case ObjectType.UnsignedByte:
                        {
                            byte index = (indexArg as IOperable<byte>).Value;
                            if (index >= array.Length)
                                throw new OperableException("Index out of range");
                            array[index] = valueArg;
                            break;
                        }
                    default:
                        throw new OperableException($"An argument of illegal type '{indexArg.OperableType}' was provided to the method");
                }
            };
            Method setMethod = new(2, set, MethodType.Consumer);
            return new MethodData(setMethod);
        }

        private static MethodDataOperable GenerateEnumeratorMethod(IOperable[] array)
        {
            ProviderMethod e = () =>
            {
                var enumerator = array.GetEnumerator();

                RuntimeObject obj = new();

                bool hasNext = true;

                ProviderMethod next = () =>
                {
                    if (!enumerator.MoveNext())
                    {
                        hasNext = false;
                        return BoolOperable.False;
                    }
                    return BoolOperable.True;
                };
                Method nextMethod = new(0, next, MethodType.Provider);
                MethodData nextMethodData = new(nextMethod);

                ProviderMethod current = () =>
                {
                    if (!hasNext)
                        throw new OperableException("Enumeration already finished");
                    return (IOperable)enumerator.Current;
                };
                Method currentMethod = new(0, current, MethodType.Provider);
                MethodData currentMethodData = new(currentMethod);

                obj["next"] = (MethodDataOperable)nextMethodData;
                obj["current"] = (MethodDataOperable)currentMethodData;

                return (ObjectOperable)obj;
            };
            Method eMethod = new(0, e, MethodType.Provider);
            return new MethodData(eMethod);
        }

        private static MethodDataOperable GenerateRangeMethod(IOperable[] array)
        {
            FunctionMethod range = args =>
            {
                int start = args[0].OperableType switch
                {
                    ObjectType.ArbitraryBitInteger => (int)(args[0] as IOperable<BigInteger>).Value,
                    ObjectType.UnsignedByte => (args[0] as IOperable<byte>).Value,
                    _ => throw new OperableException("First argument is not a valid number type")
                };
                int stop = args[1].OperableType switch
                {
                    ObjectType.ArbitraryBitInteger => (int)(args[1] as IOperable<BigInteger>).Value,
                    ObjectType.UnsignedByte => (args[1] as IOperable<byte>).Value,
                    _ => throw new OperableException("Second argument is not a valid number type")
                };

                if (start < 0 || stop < 0)
                    throw new OperableException("Argument out of range");
                if (start > stop)
                    throw new OperableException("First argument must be smaller than the seconds argument");
                if (stop > array.Length)
                    throw new OperableException("Argument out of range");

                return (ArrayObjectOperable)array[start..stop];
            };
            Method rangeMethod = new(2, range, MethodType.Function);
            return new MethodData(rangeMethod);
        }

        private static MethodDataOperable GenerateToStringMethod(IOperable[] array)
        {
            ProviderMethod toString = () =>
            {
                StringBuilder sb = new();
                sb.Append('[');
                for (int i = 0; i < array.Length; i++)
                {
                    IOperable element = array[i];
                    switch (element.OperableType)
                    {
                        case ObjectType.ArrayObject:
                            {
                                var arrObj = element as ArrayObjectOperable;
                                if (arrObj._array == array)
                                {
                                    sb.Append("<this array>");
                                    break;
                                }

                                if (!arrObj.Value.TryGetMember("toString", out var ts))
                                {
                                    sb.Append(arrObj.ToString());
                                    break;
                                }
                                Method tsMethod = (ts as MethodDataOperable).Value.GetOverload(0);
                                sb.Append(tsMethod.GetProvider().Invoke().ToString());
                                break;
                            }
                        case ObjectType.StringObject:
                            {
                                sb.Append('"')
                                    .Append(EscapeSpecialCharacters(element.ToString()))
                                    .Append('"');
                                break;
                            }
                        case ObjectType.Utf32Character:
                            {
                                sb.Append('\'')
                                    .Append(EscapeSpecialCharacters(element.ToString()))
                                    .Append('\'');
                                break;
                            }
                        default:
                            sb.Append(element.ToString());
                            break;
                    }

                    if (i != array.Length - 1)
                        sb.Append(',');
                }
                sb.Append(']');
                return (StringObjectOperable)sb.ToString();
            };
            Method toStringMethod = new(0, toString, MethodType.Provider);
            return new MethodData(toStringMethod);
        }

        private static MethodDataOperable GenerateOperatorEquals()
        {
            FunctionMethod operatorEquals = args =>
            {
                IBinaryOperable left = (IBinaryOperable)args[0];
                IOperable right = args[1];

                return left.Equal(right);
            };
            Method operatorEqualsMethod = new(2, operatorEquals, MethodType.Function);
            return new MethodData(operatorEqualsMethod);
        }

        private static MethodDataOperable GenerateOperatorNotEquals()
        {
            FunctionMethod operatorNotEquals = args =>
            {
                IBinaryOperable left = (IBinaryOperable)args[0];
                IOperable right = args[1];

                return left.NotEqual(right);
            };
            Method operatorNotEqualsMethod = new(2, operatorNotEquals, MethodType.Function);
            return new MethodData(operatorNotEqualsMethod);
        }

        private static MethodDataOperable GenerateToObjectMethod(RuntimeObject obj)
        {
            ProviderMethod toObject = () => (ObjectOperable)obj;
            Method toObjectMethod = new(0, toObject, MethodType.Provider);
            return new MethodData(toObjectMethod);
        }

        #endregion

        private static string EscapeSpecialCharacters(string value)
        {
            value = value.Replace("\\", "\\\\");
            value = value.Replace("\"", "\\\"");
            value = value.Replace("\n", "\\n");
            value = value.Replace("\t", "\\t");
            return value;
        }

        public override IOperable Equal(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArrayObject => StrictEqual(operand),
                ObjectType.Void => BoolOperable.False,
                _ => throw MissingBinaryOperatorImplementation(operand, Models.Enums.BinaryOperator.Equal)
            };
        }

        public override IOperable NotEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArrayObject => StrictNotEqual(operand),
                ObjectType.Void => BoolOperable.True,
                _ => throw MissingBinaryOperatorImplementation(operand, Models.Enums.BinaryOperator.NotEqual)
            };
        }

        public override IOperable<bool> StrictEqual(IOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BoolOperable.False;

            return BoolOperable.FromBool(Value == operand.Value);
        }

        public override IOperable<bool> StrictNotEqual(IOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BoolOperable.True;

            return BoolOperable.FromBool(Value != operand.Value);
        }

        public override string ToString()
        {
            if (!Value.TryGetMember(METHOD_NAME_TO_STRING, out var ts))
                return base.ToString();

            Method tsMethod = (ts as IOperable<MethodData>).Value.GetOverload(0);
            return tsMethod.GetProvider().Invoke().ToString();
        }

        public static ArrayObjectOperable Allocate(ulong size) => Allocate(VoidOperable.Void, size).ToArray();

        public static ArrayObjectOperable Allocate(int size) => Allocate(VoidOperable.Void, size).ToArray();

        public static ArrayObjectOperable Allocate(byte size) => Allocate(VoidOperable.Void, size).ToArray();

        private static IEnumerable<IOperable> Allocate(IOperable initial, ulong size)
        {
            for (ulong i = 0; i < size; i++)
                yield return initial;
        }

        private static IEnumerable<IOperable> Allocate(IOperable initial, int size)
        {
            for (int i = 0; i < size; i++)
                yield return initial;
        }

        private static IEnumerable<IOperable> Allocate(IOperable initial, byte size)
        {
            for (byte i = 0; i < size; i++)
                yield return initial;
        }

        public static implicit operator ArrayObjectOperable(IOperable[] value) => new(value);
    }
}
