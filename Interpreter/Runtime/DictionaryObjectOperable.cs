using Interpreter.Environment;
using Interpreter.Models.Delegates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Runtime
{
    public class DictionaryObjectOperable : OperableBase<RuntimeObject>
    {
        #region private constants
        private const string METHOD_NAME_GET = "get";
        private const string METHOD_NAME_SET = "set";
        private const string METHOD_NAME_CONTAINS = "contains";
        private const string METHOD_NAME_REMOVE = "remove";
        private const string METHOD_NAME_LENGTH = "length";
        private const string METHOD_NAME_KEYS = "keys";
        private const string METHOD_NAME_VALUES = "values";
        private const string METHOD_NAME_ENUMERATOR = "enumerator";
        private const string METHOD_NAME_TO_STRING = "toString";
        private const string METHOD_NAME_INDEXER_GET = "__indexer_get__";
        private const string METHOD_NAME_INDEXER_SET = "__indexer_set__";
        private const string METHOD_NAME_OPERATOR_ADD = "__operator_add__";
        #endregion

        private readonly IDictionary<IOperable, IOperable> _dictionary;

        public DictionaryObjectOperable() : this(new Dictionary<IOperable, IOperable>())
        {
        }

        public DictionaryObjectOperable(IDictionary<IOperable, IOperable> dictionary) : base(GenerateRuntimeObject(dictionary), ObjectType.DictionaryObject)
        {
            _dictionary = dictionary;
        }

        private static RuntimeObject GenerateRuntimeObject(IDictionary<IOperable, IOperable> dictionary)
        {
            RuntimeObject obj = new();

            var indexGet = GenerateGetMethod(dictionary);
            var indexSet = GenerateSetMethod(dictionary);

            obj[METHOD_NAME_GET] = indexGet;
            obj[METHOD_NAME_SET] = indexSet;
            obj[METHOD_NAME_CONTAINS] = GenerateContainsMethod(dictionary);
            obj[METHOD_NAME_REMOVE] = GenerateRemoveMethod(dictionary);
            obj[METHOD_NAME_LENGTH] = GenerateLengthMethod(dictionary);
            obj[METHOD_NAME_KEYS] = GenerateKeysMethod(dictionary);
            obj[METHOD_NAME_VALUES] = GenerateValuesMethod(dictionary);
            obj[METHOD_NAME_ENUMERATOR] = GenerateEnumeratorMethod(dictionary);
            obj[METHOD_NAME_TO_STRING] = GenerateToStringMethod(dictionary);
            obj[METHOD_NAME_INDEXER_GET] = indexGet;
            obj[METHOD_NAME_INDEXER_SET] = indexSet;

            return obj;
        }

        #region Method constructor methods

        private static MethodDataOperable GenerateGetMethod(IDictionary<IOperable, IOperable> dictionary)
        {
            FunctionMethod get = args =>
            {
                IOperable key = args[0];

                if (key.OperableType == ObjectType.Void)
                    throw new OperableException("Key was void");
                if (!dictionary.TryGetValue(key, out var value))
                    throw new OperableException($"Dictionary does not contain the provided key");

                return value;
            };
            Method getMethod = new(1, get, MethodType.Function);
            return new MethodData(getMethod);
        }

        private static MethodDataOperable GenerateSetMethod(IDictionary<IOperable, IOperable> dictionary)
        {
            ConsumerMethod set = args =>
            {
                IOperable key = args[0];
                IOperable value = args[1];

                if (key.OperableType == ObjectType.Void)
                    throw new OperableException("Key was void");

                dictionary[key] = value;
            };
            Method setMethod = new(2, set, MethodType.Consumer);
            return new MethodData(setMethod);
        }

        private static MethodDataOperable GenerateContainsMethod(IDictionary<IOperable, IOperable> dictionary)
        {
            FunctionMethod contains = args =>
            {
                IOperable key = args[0];

                if (key.OperableType == ObjectType.Void)
                    throw new OperableException("Key was void");

                return BoolOperable.FromBool(dictionary.ContainsKey(key));
            };
            Method containsMethod = new(1, contains, MethodType.Function);
            return new MethodData(containsMethod);
        }

        private static MethodDataOperable GenerateRemoveMethod(IDictionary<IOperable, IOperable> dictionary)
        {
            FunctionMethod remove = args =>
            {
                IOperable key = args[0];

                if (key.OperableType == ObjectType.Void)
                    throw new OperableException("Key was void");

                return BoolOperable.FromBool(dictionary.Remove(key));
            };
            Method removeMethod = new(1, remove, MethodType.Function);
            return new MethodData(removeMethod);
        }

        private static MethodDataOperable GenerateLengthMethod(IDictionary<IOperable, IOperable> dictionary)
        {
            ProviderMethod length = () => (BigIntOperable)dictionary.Count;
            Method lengthMethod = new(0, length, MethodType.Provider);
            return new MethodData(lengthMethod);
        }

        private static MethodDataOperable GenerateKeysMethod(IDictionary<IOperable, IOperable> dictionary)
        {
            ProviderMethod keys = () => (ArrayObjectOperable)dictionary.Keys;
            Method keysMethod = new(0, keys, MethodType.Provider);
            return new MethodData(keysMethod);
        }

        private static MethodDataOperable GenerateValuesMethod(IDictionary<IOperable, IOperable> dictionary)
        {
            ProviderMethod values = () => (ArrayObjectOperable)dictionary.Values;
            Method valuesMethod = new(0, values, MethodType.Provider);
            return new MethodData(valuesMethod);
        }

        private static MethodDataOperable GenerateEnumeratorMethod(IDictionary<IOperable, IOperable> dictionary)
        {
            ProviderMethod e = () =>
            {
                IEnumerator<KeyValuePair<IOperable, IOperable>> enumerator = dictionary.GetEnumerator();

                RuntimeObject obj = new();

                bool hasNext = true;
                bool isDestructed = false;
                ProviderMethod next = () =>
                {
                    if (isDestructed)
                        throw new OperableException("Enumerator is destructed");
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
                    if (isDestructed)
                        throw new OperableException("Enumerator is destructed");
                    if (!hasNext)
                        throw new OperableException("Enumeration already finished");

                    var kvp = enumerator.Current;

                    RuntimeObject kvpObj = new();
                    kvpObj["key"] = kvp.Key;
                    kvpObj["value"] = kvp.Value;

                    return (ObjectOperable)kvpObj;
                };
                Method currentMethod = new(0, current, MethodType.Provider);
                MethodData currentMethodData = new(currentMethod);

                ActionMethod destruct = () =>
                {
                    if (!isDestructed)
                    {
                        enumerator.Dispose();
                        isDestructed = true;
                    }
                };
                Method destructMethod = new(0, destruct, MethodType.Action);
                MethodData destructMethodData = new(destructMethod);

                obj["next"] = (MethodDataOperable)nextMethodData;
                obj["current"] = (MethodDataOperable)currentMethodData;
                obj["destruct"] = (MethodDataOperable)destructMethodData;

                return (ObjectOperable)obj;
            };
            Method eMethod = new(0, e, MethodType.Provider);
            return new MethodData(eMethod);
        }

        private static MethodDataOperable GenerateToStringMethod(IDictionary<IOperable, IOperable> dictionary)
        {
            ProviderMethod toString = () =>
            {
                using var enumerator = dictionary.GetEnumerator();
                StringBuilder sb = new();
                sb.Append("dict{");
                int i = 0;
                while (enumerator.MoveNext())
                {
                    sb.Append('[');
                    var kvp = enumerator.Current;
                    IOperable key = kvp.Key;
                    IOperable value = kvp.Value;
                    switch (key.OperableType)
                    {
                        case ObjectType.DictionaryObject:
                            {
                                var dict = (DictionaryObjectOperable)key;
                                if (dict._dictionary == dictionary)
                                {
                                    sb.Append("<this dict>");
                                    break;
                                }
                                if (!dict.Value.TryGetMember("toString", out var ts))
                                {
                                    sb.Append(dict.ToString());
                                    break;
                                }
                                Method tsMethod = (ts as IOperable<MethodData>).Value.GetOverload(0);
                                sb.Append(tsMethod.GetProvider().Invoke().ToString());
                                break;
                            }
                        case ObjectType.StringObject:
                            {
                                sb.Append('"')
                                    .Append(EscapeSpecialCharacters(key.ToString()))
                                    .Append('"');
                                break;
                            }
                        case ObjectType.Utf32Character:
                            {
                                sb.Append('\'')
                                    .Append(EscapeSpecialCharacters(key.ToString()))
                                    .Append('\'');
                                break;
                            }
                        default:
                            sb.Append(key.ToString());
                            break;
                    }
                    sb.Append(']');
                    sb.Append('=');
                    switch (value.OperableType)
                    {
                        case ObjectType.DictionaryObject:
                            {
                                var dict = (DictionaryObjectOperable)value;
                                if (dict._dictionary == dictionary)
                                {
                                    sb.Append("<this dict>");
                                    break;
                                }
                                if (!dict.Value.TryGetMember("toString", out var ts))
                                {
                                    sb.Append(dict.ToString());
                                    break;
                                }
                                Method tsMethod = (ts as IOperable<MethodData>).Value.GetOverload(0);
                                sb.Append(tsMethod.GetProvider().Invoke().ToString());
                                break;
                            }
                        case ObjectType.StringObject:
                            {
                                sb.Append('"')
                                    .Append(EscapeSpecialCharacters(value.ToString()))
                                    .Append('"');
                                break;
                            }
                        case ObjectType.Utf32Character:
                            {
                                sb.Append('\'')
                                    .Append(EscapeSpecialCharacters(value.ToString()))
                                    .Append('\'');
                                break;
                            }
                        default:
                            sb.Append(value.ToString());
                            break;
                    }

                    if (i != dictionary.Count - 1)
                        sb.Append(',');

                    i++;
                }
                sb.Append('}');
                return (StringObjectOperable)sb.ToString();
            };
            Method toStringMethod = new(0, toString, MethodType.Provider);
            return new MethodData(toStringMethod);
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
                ObjectType.DictionaryObject => StrictEqual(operand),
                ObjectType.Void => BoolOperable.False,
                _ => throw MissingBinaryOperatorImplementation(operand, Models.Enums.BinaryOperator.Equal)
            };
        }

        public override IOperable NotEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.DictionaryObject => StrictNotEqual(operand),
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
            if (!Value.TryGetMember("toString", out var ts))
                return base.ToString();

            Method tsMethod = (ts as IOperable<MethodData>).Value.GetOverload(0);
            return tsMethod.GetProvider().Invoke().ToString();
        }

        public static implicit operator DictionaryObjectOperable(Dictionary<IOperable, IOperable> value) => new(value);
    }
}
