using Interpreter.Models.Enums;
using Interpreter.Types;
using Singulink.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Runtime
{
    public class BigDecimalOperable : OperableBase<BigDecimal>
    {
        private const string DECIMAL_FORMAT = "G"; // General

        private static readonly System.Globalization.CultureInfo DECIMAL_CULTURE = 
            System.Globalization.CultureInfo.InvariantCulture;

        public BigDecimalOperable(BigDecimal value) : base(value, ObjectType.ArbitraryPrecisionDecimal)
        {
        }

        #region Binary Operator

        public override IOperable Add(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => (BigDecimalOperable)(Value + (operand as IOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => (BigDecimalOperable)(Value + (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => (BigDecimalOperable)(Value + (uint)(operand as IOperable<byte>).Value),
                ObjectType.String => (StringOperable)(ToString() + operand.ToString()),
                ObjectType.StringObject => (StringObjectOperable)(ToString() + operand.ToString()),
                ObjectType.Utf32Character => (BigDecimalOperable)(Value + (operand as IOperable<Utf32Character>).Value.Value),
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Add)
            };
        }

        public override IOperable Divide(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => (BigDecimalOperable)(Value / (operand as IOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => Value / (operand as IOperable<BigInteger>).Value,
                ObjectType.UnsignedByte => Value / (uint)(operand as IOperable<byte>).Value,
                ObjectType.Utf32Character => Value / (operand as IOperable<Utf32Character>).Value.Value,
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Div)
            };
        }

        public override IOperable Equal(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool(Value == (operand as IOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value == (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value == (uint)(operand as IOperable<byte>).Value),
                ObjectType.Utf32Character => BoolOperable.FromBool(Value == (operand as IOperable<Utf32Character>).Value.Value),
                ObjectType.Void => BoolOperable.False,
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Equal)
            };
        }

        public override IOperable GreaterThan(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool(Value > (operand as IOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value > (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value > (uint)(operand as IOperable<byte>).Value),
                ObjectType.Utf32Character => BoolOperable.FromBool(Value > (operand as IOperable<Utf32Character>).Value.Value),
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.GreaterThan)
            };
        }

        public override IOperable GreaterThanOrEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool(Value >= (operand as IOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value >= (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value >= (uint)(operand as IOperable<byte>).Value),
                ObjectType.Utf32Character => BoolOperable.FromBool(Value >= (operand as IOperable<Utf32Character>).Value.Value),
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.GreaterThanOrEqual)
            };
        }

        public override IOperable LessThan(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool(Value < (operand as IOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value < (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value < (uint)(operand as IOperable<byte>).Value),
                ObjectType.Utf32Character => BoolOperable.FromBool(Value < (operand as IOperable<Utf32Character>).Value.Value),
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.LessThan)
            };
        }

        public override IOperable LessThanOrEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool(Value <= (operand as IOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value <= (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value <= (uint)(operand as IOperable<byte>).Value),
                ObjectType.Utf32Character => BoolOperable.FromBool(Value <= (operand as IOperable<Utf32Character>).Value.Value),
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.LessThanOrEqual)
            };
        }

        public override IOperable Mod(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => (BigDecimalOperable)(Value % (operand as IOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => Value % (operand as IOperable<BigInteger>).Value,
                ObjectType.UnsignedByte => Value % (uint)(operand as IOperable<byte>).Value,
                ObjectType.Utf32Character => Value % (operand as IOperable<Utf32Character>).Value.Value,
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Mod)
            };
        }

        public override IOperable Multiply(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => (BigDecimalOperable)(Value * (operand as IOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => Value * (operand as IOperable<BigInteger>).Value,
                ObjectType.UnsignedByte => Value * (uint)(operand as IOperable<byte>).Value,
                ObjectType.Utf32Character => Value * (operand as IOperable<Utf32Character>).Value.Value,
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Mult)
            };
        }

        public override IOperable NotEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool(Value != (operand as IOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value != (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value != (uint)(operand as IOperable<byte>).Value),
                ObjectType.Utf32Character => BoolOperable.FromBool(Value != (operand as IOperable<Utf32Character>).Value.Value),
                ObjectType.Void => BoolOperable.True,
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.NotEqual)
            };
        }

        public override IOperable Subtract(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => (BigDecimalOperable)(Value - (operand as IOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => Value - (operand as IOperable<BigInteger>).Value,
                ObjectType.UnsignedByte => Value - (uint)(operand as IOperable<byte>).Value,
                ObjectType.Utf32Character => Value - (operand as IOperable<Utf32Character>).Value.Value,
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Sub)
            };
        }

        public override IOperable<bool> StrictEqual(IOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BoolOperable.False;

            return BoolOperable.FromBool(Value == (BigDecimal)operand.Value);
        }

        public override IOperable<bool> StrictNotEqual(IOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BoolOperable.True;

            return BoolOperable.FromBool(Value != (BigDecimal)operand.Value);
        }

        #endregion

        #region Unary Operator

        public override IOperable UnaryMinus() => (BigDecimalOperable)(-Value);

        #endregion

        public static implicit operator BigDecimalOperable(BigDecimal value) => new(value);

        public override string ToString() => Value.ToString(DECIMAL_FORMAT, DECIMAL_CULTURE);

        #region Convertible Implementations
        public override TypeCode GetTypeCode() => TypeCode.Object;
        public override bool ToBoolean(IFormatProvider provider) => Value != 0;
        public override BigDecimal ToBigDecimal(IFormatProvider provider) => Value;
        public override BigInteger ToBigInteger(IFormatProvider provider) => (BigInteger)Value;
        public override byte ToByte(IFormatProvider provider) => (byte)(int)Value;
        public override char ToChar(IFormatProvider provider) => (char)(int)Value;
        public override decimal ToDecimal(IFormatProvider provider) => (decimal)Value;
        public override double ToDouble(IFormatProvider provider) => (double)Value;
        public override short ToInt16(IFormatProvider provider) => (short)Value;
        public override int ToInt32(IFormatProvider provider) => (int)Value;
        public override long ToInt64(IFormatProvider provider) => (long)Value;
        public override sbyte ToSByte(IFormatProvider provider) => (sbyte)(int)Value;
        public override float ToSingle(IFormatProvider provider) => (float)Value;
        public override ushort ToUInt16(IFormatProvider provider) => (ushort)(uint)Value;
        public override uint ToUInt32(IFormatProvider provider) => (uint)Value;
        public override ulong ToUInt64(IFormatProvider provider) => (ulong)Value;
        public override string ToString(IFormatProvider provider) => Value.ToString(DECIMAL_FORMAT, provider);
        #endregion
    }
}
