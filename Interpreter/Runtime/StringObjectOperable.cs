using Interpreter.Environment;
using Interpreter.Models.Delegates;
using Interpreter.Runtime.Extensions;
using Interpreter.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Runtime
{
    public class StringObjectOperable : OperableBase<RuntimeObject>
    {
        #region Private constants
        private const string METHOD_NAME_TO_STRING              = "toString";
        private const string METHOD_NAME_LENGTH                 = "length";
        private const string METHOD_NAME_REPLACE_ALL            = "replaceAll";
        private const string METHOD_NAME_SPLIT                  = "split";
        private const string METHOD_NAME_ENUMERATOR             = "enumerator";
        private const string METHOD_NAME_INDEXER_GET            = "__indexer_get__";
        private const string METHOD_NAME_OPERATOR_ADD           = "__operator_add__";
        private const string METHOD_NAME_OPERATOR_EQUALS        = "__operator_equals__";
        private const string METHOD_NAME_OPERATOR_NOT_EQUALS    = "__operator_not_equals__";
        private const string METHOD_NAME_TO_OBJECT              = "toObject";
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
            obj[METHOD_NAME_ENUMERATOR] = GenerateStringEnumeratorMethod(value);
            obj[METHOD_NAME_INDEXER_GET] = GenerateIndexerGetMethod(value);
            obj[METHOD_NAME_TO_OBJECT] = GenerateToObjectMethod(obj);

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
                            return new ArrayOperable(segments.Select(s => (StringObjectOperable)s).ToArray());
                        }
                    case ObjectType.Utf32Character:
                        {
                            Utf32Character tmp = (arg as IOperable<Utf32Character>).Value;
                            string[] segments = value.Split((char)tmp.Value);
                            return new ArrayOperable(segments.Select(s => (StringObjectOperable)s).ToArray());
                        }
                    default:
                        throw new OperableException($"An argument with illegal type '{arg.OperableType}' was provided");
                }
            };
            Method splitMethod = new(1, split, MethodType.Function);
            return new MethodData(splitMethod);
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
    }
}
