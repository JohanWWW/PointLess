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
    public class ByteOperable : OperableBase<byte>
    {
        public static readonly ByteOperable MinValue = new(byte.MinValue);
        public static readonly ByteOperable MaxValue = new(byte.MaxValue);

        public ByteOperable(byte value) : base(value, ObjectType.UnsignedByte)
        {
        }

        public ByteOperable(int value) : this((byte)value)
        {
        }

        public override IOperable Add(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => (ByteOperable)(Value + (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => (BigIntOperable)(Value + (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => (BigDecimalOperable)((int)Value + (operand as IOperable<BigDecimal>).Value),
                ObjectType.Utf32Character => (BigIntOperable)(Value + (operand as IOperable<Utf32Character>).Value.Value),
                ObjectType.String => (StringOperable)(ToString() + operand.ToString()),
                ObjectType.StringObject => (StringObjectOperable)(ToString() + operand.ToString()),
                _ => throw MissingBinaryOperatorImplementation(operand, Models.Enums.BinaryOperator.Add)
            };
        }

        public override IOperable Subtract(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => (ByteOperable)(Value - (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => (BigIntOperable)(Value - (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => (BigDecimalOperable)((uint)Value - (operand as IOperable<BigDecimal>).Value),
                ObjectType.Utf32Character => (BigIntOperable)(Value - (operand as IOperable<Utf32Character>).Value.Value),
                _ => throw MissingBinaryOperatorImplementation(operand, Models.Enums.BinaryOperator.Sub)
            };
        }

        public override IOperable Multiply(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => (ByteOperable)(Value * (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => (BigIntOperable)(Value * (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => (BigDecimalOperable)((uint)Value * (operand as IOperable<BigDecimal>).Value),
                ObjectType.Utf32Character => (BigIntOperable)(Value * (operand as IOperable<Utf32Character>).Value.Value),
                _ => throw MissingBinaryOperatorImplementation(operand, Models.Enums.BinaryOperator.Mult)
            };
        }

        public override IOperable Divide(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => (ByteOperable)(Value / (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => (BigIntOperable)(Value / (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => (BigDecimalOperable)((uint)Value / (operand as IOperable<BigDecimal>).Value),
                ObjectType.Utf32Character => (BigIntOperable)(Value / (operand as IOperable<Utf32Character>).Value.Value),
                _ => throw MissingBinaryOperatorImplementation(operand, Models.Enums.BinaryOperator.Div)
            };
        }

        public override IOperable Mod(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => (ByteOperable)(Value % (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => (BigIntOperable)(Value % (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => (BigDecimalOperable)((uint)Value % (operand as IOperable<BigDecimal>).Value),
                ObjectType.Utf32Character => (BigIntOperable)(Value % (operand as IOperable<Utf32Character>).Value.Value),
                _ => throw MissingBinaryOperatorImplementation(operand, Models.Enums.BinaryOperator.Mod)
            };
        }

        public override IOperable BitwiseAnd(Func<IOperable> operand)
        {
            IOperable eval = operand();

            return eval.OperableType switch
            {
                ObjectType.UnsignedByte => (ByteOperable)(Value & (eval as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => (BigIntOperable)(Value & (eval as IOperable<BigInteger>).Value),
                ObjectType.Utf32Character => (BigIntOperable)(Value & (eval as IOperable<Utf32Character>).Value.Value),
                _ => throw MissingBinaryOperatorImplementation(eval, Models.Enums.BinaryOperator.BitwiseAnd)
            };
        }

        public override IOperable BitwiseOr(Func<IOperable> operand)
        {
            IOperable eval = operand();

            return eval.OperableType switch
            {
                ObjectType.UnsignedByte => (ByteOperable)(Value | (eval as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => (BigIntOperable)(Value | (eval as IOperable<BigInteger>).Value),
                ObjectType.Utf32Character => (BigIntOperable)(Value | (eval as IOperable<Utf32Character>).Value.Value),
                _ => throw MissingBinaryOperatorImplementation(eval, Models.Enums.BinaryOperator.BitwiseOr)
            };
        }

        public override IOperable BitwiseXOr(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => (ByteOperable)(Value ^ (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => (BigIntOperable)(Value ^ (operand as IOperable<BigInteger>).Value),
                ObjectType.Utf32Character => (BigIntOperable)(Value ^ (operand as IOperable<Utf32Character>).Value.Value),
                _ => throw MissingBinaryOperatorImplementation(operand, Models.Enums.BinaryOperator.BitwiseXOr)
            };
        }

        public override IOperable ShiftLeft(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => (ByteOperable)(Value << (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => (ByteOperable)(Value << (int)(operand as IOperable<BigInteger>).Value),
                ObjectType.Utf32Character => (ByteOperable)(Value << (operand as IOperable<Utf32Character>).Value.Value),
                _ => throw MissingBinaryOperatorImplementation(operand, Models.Enums.BinaryOperator.ShiftLeft)
            };
        }

        public override IOperable ShiftRight(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => (ByteOperable)(Value >> (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => (ByteOperable)(Value >> (int)(operand as IOperable<BigInteger>).Value),
                ObjectType.Utf32Character => (ByteOperable)(Value >> (operand as IOperable<Utf32Character>).Value.Value),
                _ => throw MissingBinaryOperatorImplementation(operand, Models.Enums.BinaryOperator.ShiftRight)
            };
        }

        public override IOperable Equal(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value == (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value == (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool((uint)Value == (operand as IOperable<BigDecimal>).Value),
                ObjectType.Utf32Character => BoolOperable.FromBool(Value == (operand as IOperable<Utf32Character>).Value.Value),
                _ => throw MissingBinaryOperatorImplementation(operand, Models.Enums.BinaryOperator.Equal)
            };
        }

        public override IOperable NotEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value != (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value != (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool((uint)Value != (operand as IOperable<BigDecimal>).Value),
                ObjectType.Utf32Character => BoolOperable.FromBool(Value != (operand as IOperable<Utf32Character>).Value.Value),
                _ => throw MissingBinaryOperatorImplementation(operand, Models.Enums.BinaryOperator.NotEqual)
            };
        }

        public override IOperable GreaterThan(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value > (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value > (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool((uint)Value > (operand as IOperable<BigDecimal>).Value),
                ObjectType.Utf32Character => BoolOperable.FromBool(Value > (operand as IOperable<Utf32Character>).Value.Value),
                _ => throw MissingBinaryOperatorImplementation(operand, Models.Enums.BinaryOperator.GreaterThan)
            };
        }

        public override IOperable GreaterThanOrEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value >= (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value >= (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool((uint)Value >= (operand as IOperable<BigDecimal>).Value),
                ObjectType.Utf32Character => BoolOperable.FromBool(Value >= (operand as IOperable<Utf32Character>).Value.Value),
                _ => throw MissingBinaryOperatorImplementation(operand, Models.Enums.BinaryOperator.GreaterThanOrEqual)
            };
        }

        public override IOperable LessThan(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value < (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value < (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool((uint)Value < (operand as IOperable<BigDecimal>).Value),
                ObjectType.Utf32Character => BoolOperable.FromBool(Value < (operand as IOperable<Utf32Character>).Value.Value),
                _ => throw MissingBinaryOperatorImplementation(operand, Models.Enums.BinaryOperator.LessThan)
            };
        }

        public override IOperable LessThanOrEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value <= (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value <= (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool((uint)Value <= (operand as IOperable<BigDecimal>).Value),
                ObjectType.Utf32Character => BoolOperable.FromBool(Value <= (operand as IOperable<Utf32Character>).Value.Value),
                _ => throw MissingBinaryOperatorImplementation(operand, Models.Enums.BinaryOperator.LessThanOrEqual)
            };
        }

        public override IOperable<bool> StrictEqual(IOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BoolOperable.False;

            return BoolOperable.FromBool(Value == (byte)operand.Value);
        }

        public override IBinaryOperable<bool> StrictNotEqual(IOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BoolOperable.True;

            return BoolOperable.FromBool(Value != (byte)operand.Value);
        }

        public static implicit operator ByteOperable(byte b) => new(b);

        public override string ToString() => "b'" + Value.ToString();
    }
}
