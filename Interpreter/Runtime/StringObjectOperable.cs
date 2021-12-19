using Interpreter.Environment;
using Interpreter.Models.Delegates;
using Interpreter.Runtime.Extensions;
using Interpreter.Types;
using Singulink.Numerics;
using System;
using System.Linq;
using System.Numerics;

namespace Interpreter.Runtime
{
    public class StringObjectOperable : OperableBase<RuntimeObject>
    {
        #region Private constants
        private const string METHOD_NAME_TO_STRING              = "toString";
        private const string METHOD_NAME_LENGTH                 = "length";
        private const string METHOD_NAME_REPLACE_ALL            = "replaceAll";
        private const string METHOD_NAME_SPLIT                  = "split";
        private const string METHOD_NAME_SUBSTRING              = "substring";
        private const string METHOD_NAME_CONTAINS               = "contains";
        private const string METHOD_NAME_TO_LOWER               = "toLower";
        private const string METHOD_NAME_TO_UPPER               = "toUpper";
        private const string METHOD_NAME_ENUMERATOR             = "enumerator";
        private const string METHOD_NAME_INDEXER_GET            = "__indexer_get__";
        private const string METHOD_NAME_OPERATOR_ADD           = "__operator_add__";
        private const string METHOD_NAME_OPERATOR_EQUALS        = "__operator_equals__";
        private const string METHOD_NAME_OPERATOR_NOT_EQUALS    = "__operator_not_equals__";
        private const string METHOD_NAME_TO_OBJECT              = "toObject";
        private const string METHOD_NAME_GET_CHARS              = "getChars";
        private const string METHOD_NAME_GET_BYTES              = "getBytes";
        #endregion

        private readonly string _stringValue;

        public StringObjectOperable(string value) : base(GenerateRuntimeObject(value), ObjectType.StringObject)
        {
            _stringValue = value;
        }

        private static RuntimeObject GenerateRuntimeObject(string value)
        {
            var obj = new RuntimeObject();           

            obj[METHOD_NAME_TO_STRING] = GenerateToStringMethod(value);
            obj[METHOD_NAME_LENGTH] = GenerateStringLengthMethod(value);
            obj[METHOD_NAME_REPLACE_ALL] = GenerateReplaceAllMethod(value);
            obj[METHOD_NAME_SPLIT] = GenerateSplitMethod(value);
            obj[METHOD_NAME_SUBSTRING] = GenerateSubstringMethod(value);
            obj[METHOD_NAME_CONTAINS] = GenerateContainsMethod(value);
            obj[METHOD_NAME_TO_LOWER] = GenerateToLowerMethod(value);
            obj[METHOD_NAME_TO_UPPER] = GenerateToUpperMethod(value);
            obj[METHOD_NAME_ENUMERATOR] = GenerateStringEnumeratorMethod(value);
            obj[METHOD_NAME_INDEXER_GET] = GenerateIndexerGetMethod(value);
            obj[METHOD_NAME_TO_OBJECT] = GenerateToObjectMethod(obj);
            obj[METHOD_NAME_GET_CHARS] = GenerateGetChars(value);
            obj[METHOD_NAME_GET_BYTES] = GenerateGetBytes(value);

            return obj;
        }

        #region Method constructor methods

        public static MethodDataOperable GenerateToStringMethod(string value)
        {
            ProviderMethod toString = () => new StringObjectOperable(value);
            Method toStringMethod = new(0, toString, MethodType.Provider);
            return new MethodData(toStringMethod);
        }

        private static MethodDataOperable GenerateStringLengthMethod(string value)
        {
            ProviderMethod length = () => (BigIntOperable)new BigInteger(value.Length);
            Method lengthMethod = new(0, length, MethodType.Provider);
            return new MethodData(lengthMethod);
        }

        private static MethodDataOperable GenerateStringEnumeratorMethod(string value)
        {
            ProviderMethod e = () =>
            {
                var readOnlyEnumerator = value.GetEnumerator();

                RuntimeObject enumeratorObj = new();

                bool hasNext = true;
                ProviderMethod next = () =>
                {
                    if (!readOnlyEnumerator.MoveNext())
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
                    return (CharacterOperable)readOnlyEnumerator.Current;
                };
                Method currentMethod = new(0, current, MethodType.Provider);
                MethodData currentMethodData = new(currentMethod);

                enumeratorObj["next"] = (MethodDataOperable)nextMethodData;
                enumeratorObj["current"] = (MethodDataOperable)currentMethodData;

                return (ObjectOperable)enumeratorObj;
            };
            Method eMethod = new(0, e, MethodType.Provider);
            return new MethodData(eMethod);
        }

        private static MethodDataOperable GenerateIndexerGetMethod(string value)
        {
            FunctionMethod indexerGet = args =>
            {
                IOperable index = args[0];

                switch (index.OperableType)
                {
                    case ObjectType.ArbitraryBitInteger:
                        { 
                            BigInteger tmp = (index as IOperable<BigInteger>).Value;
                            if (tmp < 0 || tmp > int.MaxValue)
                                throw new OperableException($"Index out of legal range");
                            if (tmp >= value.Length)
                                throw new OperableException($"Index was greater than or equal to the character length");
                            return (CharacterOperable)value[(int)tmp];
                        }
                    case ObjectType.UnsignedByte:
                        {
                            byte tmp = (index as IOperable<byte>).Value;
                            if (tmp >= value.Length)
                                throw new OperableException($"Index was greater than or equal to the character length");
                            return (CharacterOperable)value[tmp];
                        }
                    case ObjectType.Utf32Character:
                        {
                            Utf32Character tmp = (index as IOperable<Utf32Character>).Value;
                            if (tmp.Value < 0)
                                throw new OperableException("Index out of legal range");
                            return (CharacterOperable)value[tmp.Value];
                        }
                    default:
                        throw new OperableException($"An argument of illegal type '{index.OperableType}' was provided to the indexer");
                }
            };
            Method indexerGetMethod = new(1, indexerGet, MethodType.Function);
            return new MethodData(indexerGetMethod);
        }

        private static MethodDataOperable GenerateReplaceAllMethod(string value)
        {
            FunctionMethod replaceAll = args =>
            {
                IOperable arg1 = args[0];
                IOperable arg2 = args[1];

                if (arg1.OperableType != ObjectType.StringObject || arg2.OperableType != ObjectType.StringObject)
                    throw new OperableException("One of the provided arguments was not a string object");

                string oldSequence = (arg1 as StringObjectOperable)._stringValue;
                string newSequence = (arg2 as StringObjectOperable)._stringValue;

                return (StringObjectOperable)value.Replace(oldSequence, newSequence);
            };
            Method replaceAllMethod = new(2, replaceAll, MethodType.Function);
            return new MethodData(replaceAllMethod);
        }

        private static MethodDataOperable GenerateSplitMethod(string value)
        {
            FunctionMethod split = args =>
            {
                IOperable arg = args[0];

                switch (arg.OperableType)
                {
                    case ObjectType.StringObject:
                        {
                            string tmp = (arg as StringObjectOperable)._stringValue;
                            string[] segments = value.Split(tmp);
                            return new ArrayObjectOperable(segments.Select(s => (StringObjectOperable)s).ToArray());
                        }
                    case ObjectType.Utf32Character:
                        {
                            Utf32Character tmp = (arg as IOperable<Utf32Character>).Value;
                            string[] segments = value.Split((char)tmp.Value);
                            return new ArrayObjectOperable(segments.Select(s => (StringObjectOperable)s).ToArray());
                        }
                    default:
                        throw new OperableException($"An argument with illegal type '{arg.OperableType}' was provided");
                }
            };
            Method splitMethod = new(1, split, MethodType.Function);
            return new MethodData(splitMethod);
        }

        private static MethodDataOperable GenerateSubstringMethod(string value)
        {
            FunctionMethod substring = args =>
            {
                IOperable startArg = args[0];
                IOperable stopArg = args[1];

                int start = startArg.OperableType switch
                {
                    ObjectType.ArbitraryBitInteger => (int)(startArg as IOperable<BigInteger>).Value,
                    ObjectType.UnsignedByte => (startArg as IOperable<byte>).Value,
                    _ => throw new OperableException("First argument is not a valid number type"),
                };
                int stop = stopArg.OperableType switch
                {
                    ObjectType.ArbitraryBitInteger => (int)(stopArg as IOperable<BigInteger>).Value,
                    ObjectType.UnsignedByte => (stopArg as IOperable<byte>).Value,
                    _ => throw new OperableException("Second argument is not a valid number type"),
                };

                if (start < 0 || stop < 0)
                    throw new OperableException("Argument out of range");
                if (start > stop)
                    throw new OperableException("First argument must be smaller than second argument");
                if (stop > value.Length)
                    throw new OperableException("Argument out of range");

                return (StringObjectOperable)value[start..stop];
            };
            Method substringMethod = new(2, substring, MethodType.Function);
            return new MethodData(substringMethod);
        }

        private static MethodDataOperable GenerateContainsMethod(string value)
        {
            FunctionMethod contains = args =>
            {
                return args[0].OperableType switch
                {
                    ObjectType.StringObject => BoolOperable.FromBool(value.Contains((args[0] as StringObjectOperable)._stringValue)),
                    ObjectType.String => BoolOperable.FromBool(value.Contains((args[0] as IOperable<string>).Value)),
                    ObjectType.Utf32Character => BoolOperable.FromBool(value.Contains((char)(args[0] as IOperable<Utf32Character>).Value.Value)),
                    _ => throw new OperableException("Method requires an argument of type string"),
                };
            };
            Method containsMethod = new(1, contains, MethodType.Function);
            return new MethodData(containsMethod);
        }

        private static MethodDataOperable GenerateToLowerMethod(string value)
        {
            ProviderMethod toLower = () => (StringObjectOperable)value.ToLower();
            Method toLowerMethod = new(0, toLower, MethodType.Provider);
            return new MethodData(toLowerMethod);
        }

        private static MethodDataOperable GenerateToUpperMethod(string value)
        {
            ProviderMethod toUpper = () => (StringObjectOperable)value.ToUpper();
            Method toUpperMethod = new(0, toUpper, MethodType.Provider);
            return new MethodData(toUpperMethod);
        }

        private static MethodDataOperable GenerateOperatorAddMethod()
        {
            FunctionMethod operatorAdd = args =>
            {
                IBinaryOperable left = (IBinaryOperable)args[0];
                IOperable right = args[1];

                return left.Add(right);
            };
            Method operatorAddMethod = new(2, operatorAdd, MethodType.Function);
            return new MethodData(operatorAddMethod);
        }

        private static MethodDataOperable GenerateOperatorEqualsMethod()
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

        private static MethodDataOperable GenerateOperatorNotEqualsMethod()
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

        private static MethodDataOperable GenerateGetChars(string value)
        {
            ProviderMethod getChars = () => new ArrayObjectOperable(value.Select(c => (CharacterOperable)c).ToArray());
            Method getCharsMethod = new(0, getChars, MethodType.Provider);
            return new MethodData(getCharsMethod);
        }

        private static MethodDataOperable GenerateGetBytes(string value)
        {
            ProviderMethod getBytes = () =>
            {
                IOperable[] bytes = new IOperable[value.Length * 4];
                for (int i = 0, b = 0; i < value.Length; i++, b += 4)
                {
                    int c = value[i];

                    bytes[b]        = (ByteOperable)(c & 0xff);
                    bytes[b + 1]    = (ByteOperable)((c >> 8) & 0xff);
                    bytes[b + 2]    = (ByteOperable)((c >> 16) & 0xff);
                    bytes[b + 3]    = (ByteOperable)(c >> 21);
                }
                return (ArrayObjectOperable)bytes;
            };
            Method getBytesMethod = new(0, getBytes, MethodType.Provider);
            return new MethodData(getBytesMethod);
        }

        #endregion

        public override IOperable Add(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.StringObject => (StringObjectOperable)(_stringValue + (operand as StringObjectOperable)._stringValue),
                ObjectType.String       => _stringValue + (operand as IOperable<string>).Value,
                ObjectType.Utf32Character => _stringValue + (operand as IOperable<Utf32Character>).Value.ToCharString(),
                _                       => _stringValue + operand.ToString()
            };
        }

        public override IOperable Equal(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.StringObject => BoolOperable.FromBool(_stringValue == (operand as StringObjectOperable)._stringValue),
                ObjectType.String => BoolOperable.FromBool(_stringValue == (operand as IOperable<string>).Value),
                ObjectType.Void => BoolOperable.False,
                _ => throw MissingBinaryOperatorImplementation(operand, Models.Enums.BinaryOperator.Equal)
            };
        }

        public override IOperable NotEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.StringObject => BoolOperable.FromBool(_stringValue != (operand as StringObjectOperable)._stringValue),
                ObjectType.String => BoolOperable.FromBool(_stringValue != (operand as IOperable<string>).Value),
                ObjectType.Void => BoolOperable.True,
                _ => throw MissingBinaryOperatorImplementation(operand, Models.Enums.BinaryOperator.NotEqual)
            };
        }

        public override IOperable<bool> StrictEqual(IOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BoolOperable.False;

            return BoolOperable.FromBool(_stringValue == (operand as StringObjectOperable)._stringValue);
        }

        public override IOperable<bool> StrictNotEqual(IOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BoolOperable.True;

            return BoolOperable.FromBool(_stringValue == (operand as StringObjectOperable)._stringValue);
        }

        public static implicit operator StringObjectOperable(string value) => new(value);

        public override string ToString() => _stringValue;

        private bool Equals(StringObjectOperable other) => 
            _stringValue == other._stringValue;

        public override bool Equals(object obj) => Equals((StringObjectOperable)obj);

        // base.Value should not be included in hash because it has a variable hash code
        public override int GetHashCode() => HashCode.Combine(_stringValue, OperableType);

        #region Convertible Implementations
        public override TypeCode GetTypeCode() => TypeCode.String;
        public override bool ToBoolean(IFormatProvider provider) => bool.Parse(_stringValue);
        public override BigDecimal ToBigDecimal(IFormatProvider provider) => BigDecimal.Parse(_stringValue);
        public override BigInteger ToBigInteger(IFormatProvider provider) => BigInteger.Parse(_stringValue);
        public override byte ToByte(IFormatProvider provider) => byte.Parse(_stringValue);
        public override char ToChar(IFormatProvider provider) => char.Parse(_stringValue);
        public override decimal ToDecimal(IFormatProvider provider) => decimal.Parse(_stringValue);
        public override double ToDouble(IFormatProvider provider) => double.Parse(_stringValue);
        public override short ToInt16(IFormatProvider provider) => short.Parse(_stringValue);
        public override int ToInt32(IFormatProvider provider) => int.Parse(_stringValue);
        public override long ToInt64(IFormatProvider provider) => long.Parse(_stringValue);
        public override sbyte ToSByte(IFormatProvider provider) => sbyte.Parse(_stringValue);
        public override float ToSingle(IFormatProvider provider) => float.Parse(_stringValue);
        public override ushort ToUInt16(IFormatProvider provider) => ushort.Parse(_stringValue);
        public override uint ToUInt32(IFormatProvider provider) => uint.Parse(_stringValue);
        public override ulong ToUInt64(IFormatProvider provider) => ulong.Parse(_stringValue);
        public override string ToString(IFormatProvider provider) => ToString();
        #endregion
    }
}
