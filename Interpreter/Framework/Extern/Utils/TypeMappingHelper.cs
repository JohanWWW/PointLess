using Interpreter.Framework.Extern.Mapping;
using Interpreter.Runtime;
using Singulink.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using ClientType = Interpreter.Runtime.ObjectType;

namespace Interpreter.Framework.Extern.Utils
{
    public static class TypeMappingHelper
    {  

        private static readonly IReadOnlyDictionary<Type, ClientType> _mttoct = new Dictionary<Type, ClientType>
        {
            [typeof(void)]          = ClientType.Void,
            [typeof(byte)]          = ClientType.UnsignedByte,
            [typeof(sbyte)]         = ClientType.ArbitraryBitInteger,
            [typeof(ushort)]        = ClientType.ArbitraryBitInteger,
            [typeof(short)]         = ClientType.ArbitraryBitInteger,
            [typeof(uint)]          = ClientType.ArbitraryBitInteger,
            [typeof(int)]           = ClientType.ArbitraryBitInteger,
            [typeof(ulong)]         = ClientType.ArbitraryBitInteger,
            [typeof(long)]          = ClientType.ArbitraryBitInteger,
            [typeof(BigInteger)]    = ClientType.ArbitraryBitInteger,
            [typeof(float)]         = ClientType.ArbitraryPrecisionDecimal,
            [typeof(double)]        = ClientType.ArbitraryPrecisionDecimal,
            [typeof(decimal)]       = ClientType.ArbitraryPrecisionDecimal,
            [typeof(BigDecimal)]    = ClientType.ArbitraryPrecisionDecimal,
            [typeof(char)]          = ClientType.Utf32Character,
            [typeof(bool)]          = ClientType.Boolean,
            [typeof(string)]        = ClientType.StringObject,
            [typeof(object)]        = ClientType.Object,
            [typeof(IArrayAdapter)]  = ClientType.ArrayObject
        }; 

        private static readonly IReadOnlyDictionary<ClientType, Type> _cttomt = new Dictionary<ClientType, Type>
        {
            [ClientType.Void]                       = typeof(void),
            [ClientType.UnsignedByte]               = typeof(byte),
            [ClientType.ArbitraryBitInteger]        = typeof(BigInteger),
            [ClientType.ArbitraryPrecisionDecimal]  = typeof(BigDecimal),
            [ClientType.Utf32Character]             = typeof(char),
            [ClientType.Boolean]                    = typeof(bool),
            [ClientType.StringObject]               = typeof(string),
            [ClientType.Object]                     = typeof(object),
            [ClientType.ArrayObject]                = typeof(ArrayAdapter)
        };

        private static readonly IReadOnlySet<Type> _masterTypes = new HashSet<Type>
        {
            typeof(void),
            typeof(byte),
            typeof(sbyte),
            typeof(ushort),
            typeof(short),
            typeof(uint),
            typeof(int),
            typeof(ulong),
            typeof(BigInteger),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(BigDecimal),
            typeof(char),
            typeof(bool),
            typeof(string),
            typeof(object),
            typeof(IArrayAdapter)
        };

        public static bool TryGetClientType<T>(out ClientType ct) =>
            _mttoct.TryGetValue(typeof(T), out ct);

        public static bool TryGetClientType(Type mt, out ClientType ct) =>
            _mttoct.TryGetValue(mt, out ct);

        public static bool TryGetMasterType(ClientType ct, out Type mt) =>
            _cttomt.TryGetValue(ct, out mt);

        public static bool IsValidMasterType(Type type) => _masterTypes.Contains(type);

        public static object ToExternValue(IOperable clientValue, Type explicitType, Type implicitType)
        {
            if (explicitType != implicitType)
                throw new InvalidCastException($"Expected argument of type {explicitType} but found {implicitType}");
            return ToExternValue(clientValue, explicitType);
        }

        public static object ToExternValue(IOperable clientValue, Type masterType)
        {
            return masterType.Name switch
            {
                nameof(Byte) => Convert.ToByte(clientValue),
                nameof(SByte) => Convert.ToSByte(clientValue),
                nameof(UInt16) => Convert.ToUInt16(clientValue),
                nameof(Int16) => Convert.ToInt16(clientValue),
                nameof(UInt32) => Convert.ToUInt16(clientValue),
                nameof(Int32) => Convert.ToInt32(clientValue),
                nameof(UInt64) => Convert.ToUInt64(clientValue),
                nameof(Int64) => Convert.ToInt64(clientValue),
                nameof(Single) => Convert.ToSingle(clientValue),
                nameof(Double) => Convert.ToDouble(clientValue),
                nameof(Decimal) => Convert.ToDecimal(clientValue),
                nameof(BigInteger) => clientValue.ToBigInteger(null),
                nameof(BigDecimal) => clientValue.ToBigDecimal(null),
                nameof(Char) => Convert.ToChar(clientValue),
                nameof(Boolean) => Convert.ToBoolean(clientValue),
                nameof(String) => Convert.ToString(clientValue),
                nameof(IArrayAdapter) => constructArrayAdapter(clientValue as IArrayOperable),
                _ => throw new InvalidCastException(),
            };

            IArrayAdapter constructArrayAdapter(IArrayOperable clientArray)
            {
                // Get array type, for now it should be based on the first element
                if (!TryGetMasterType(clientArray.GetArray()[0].OperableType, out Type externType))
                    throw new InvalidCastException("Invalid array type");

                return new ArrayAdapter(clientArray, externType);
            }
        }

        public static IOperable ToClientValue(object extValue)
        {
            if (extValue is null)
                return VoidOperable.Void;

            return extValue.GetType().Name switch
            {
                nameof(Byte) => (ByteOperable)(byte)extValue,
                nameof(SByte) => (ByteOperable)(byte)(sbyte)extValue,
                nameof(UInt16) => new BigIntOperable((ushort)extValue),
                nameof(Int16) => new BigIntOperable((short)extValue),
                nameof(UInt32) => new BigIntOperable((uint)extValue),
                nameof(Int32) => new BigIntOperable((int)extValue),
                nameof(UInt64) => new BigIntOperable((ulong)extValue),
                nameof(Int64) => new BigIntOperable((long)extValue),
                nameof(Single) => new BigDecimalOperable((decimal)(float)extValue),
                nameof(Double) => new BigDecimalOperable((decimal)(double)extValue),
                nameof(Decimal) => new BigDecimalOperable((decimal)extValue),
                nameof(BigInteger) => (BigIntOperable)(BigInteger)extValue,
                nameof(BigDecimal) => (BigDecimalOperable)(BigDecimal)extValue,
                nameof(Char) => (CharacterOperable)(char)extValue,
                nameof(Boolean) => (BoolOperable)(bool)extValue,
                nameof(String) => (StringObjectOperable)extValue.ToString(),
                nameof(ArrayAdapter) => (extValue as IArrayAdapter).OperableReference,
                _ => throw new InvalidCastException($"Cannot cast {extValue.GetType().Name} to a client type"),
            };
        }
    }
}
