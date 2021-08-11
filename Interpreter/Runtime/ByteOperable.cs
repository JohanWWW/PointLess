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
                ObjectType.UnsignedByte => new ByteOperable(Value + (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => new BigIntOperable(Value + (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new BigDecimalOperable((int)Value + (operand as IOperable<BigDecimal>).Value),
                ObjectType.String => new StringOperable(Value.ToString() + operand.ToString()),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable Subtract(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => new ByteOperable(Value - (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => new BigIntOperable(Value - (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new BigDecimalOperable((uint)Value - (operand as IOperable<BigDecimal>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable Multiply(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => new ByteOperable(Value * (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => new BigIntOperable(Value * (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new BigDecimalOperable((uint)Value * (operand as IOperable<BigDecimal>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable Divide(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => new ByteOperable(Value / (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => new BigIntOperable(Value / (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new BigDecimalOperable((uint)Value / (operand as IOperable<BigDecimal>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable Mod(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => new ByteOperable(Value % (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => new BigIntOperable(Value % (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new BigDecimalOperable((uint)Value % (operand as IOperable<BigDecimal>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable BitwiseAnd(Func<IOperable> operand)
        {
            IOperable eval = operand();

            return eval.OperableType switch
            {
                ObjectType.UnsignedByte => new ByteOperable(Value & (eval as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable BitwiseOr(Func<IOperable> operand)
        {
            IOperable eval = operand();

            return eval.OperableType switch
            {
                ObjectType.UnsignedByte => new ByteOperable(Value | (eval as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable BitwiseXOr(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => new ByteOperable(Value ^ (operand as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable ShiftLeft(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => new ByteOperable(Value << (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => new ByteOperable(Value << (int)(operand as IOperable<BigInteger>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable ShiftRight(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => new ByteOperable(Value >> (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => new ByteOperable(Value >> (int)(operand as IOperable<BigInteger>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable Equal(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value == (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value == (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool((uint)Value == (operand as IOperable<BigDecimal>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable NotEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value != (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value != (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool((uint)Value != (operand as IOperable<BigDecimal>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable GreaterThan(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value > (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value > (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool((uint)Value > (operand as IOperable<BigDecimal>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable GreaterThanOrEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value >= (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value >= (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool((uint)Value >= (operand as IOperable<BigDecimal>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable LessThan(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value < (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value < (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool((uint)Value < (operand as IOperable<BigDecimal>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable LessThanOrEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value <= (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value <= (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool((uint)Value <= (operand as IOperable<BigDecimal>).Value),
                _ => throw new MissingOperatorOverrideException()
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

        public static implicit operator ByteOperable(byte b) => new ByteOperable(b);

        public override string ToString() => "b'" + Value.ToString();
    }
}
