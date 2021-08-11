using Singulink.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Runtime
{
    public class ByteWrapper : WrapperBase<byte>
    {
        public static readonly ByteWrapper MinValue = new(byte.MinValue);
        public static readonly ByteWrapper MaxValue = new(byte.MaxValue);

        public ByteWrapper(byte value) : base(value, ObjectType.UnsignedByte)
        {
        }

        public ByteWrapper(int value) : this((byte)value)
        {
        }

        public override IBinaryOperable Add(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => new ByteWrapper(Value + (operand as IBinaryOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value + (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper((int)Value + (operand as IBinaryOperable<BigDecimal>).Value),
                ObjectType.String => new StringWrapper(Value.ToString() + operand.ToString()),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IBinaryOperable Subtract(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => new ByteWrapper(Value - (operand as IBinaryOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value - (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper((uint)Value - (operand as IBinaryOperable<BigDecimal>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IBinaryOperable Multiply(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => new ByteWrapper(Value * (operand as IBinaryOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value * (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper((uint)Value * (operand as IBinaryOperable<BigDecimal>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IBinaryOperable Divide(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => new ByteWrapper(Value / (operand as IBinaryOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value / (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper((uint)Value / (operand as IBinaryOperable<BigDecimal>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IBinaryOperable Mod(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => new ByteWrapper(Value % (operand as IBinaryOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value % (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper((uint)Value % (operand as IBinaryOperable<BigDecimal>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IBinaryOperable BitwiseAnd(Func<IBinaryOperable> operand)
        {
            IBinaryOperable eval = operand();

            return eval.OperableType switch
            {
                ObjectType.UnsignedByte => new ByteWrapper(Value & (eval as IBinaryOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IBinaryOperable BitwiseOr(Func<IBinaryOperable> operand)
        {
            IBinaryOperable eval = operand();

            return eval.OperableType switch
            {
                ObjectType.UnsignedByte => new ByteWrapper(Value | (eval as IBinaryOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IBinaryOperable BitwiseXOr(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => new ByteWrapper(Value ^ (operand as IBinaryOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IBinaryOperable ShiftLeft(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => new ByteWrapper(Value << (operand as IBinaryOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => new ByteWrapper(Value << (int)(operand as IBinaryOperable<BigInteger>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IBinaryOperable ShiftRight(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => new ByteWrapper(Value >> (operand as IBinaryOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => new ByteWrapper(Value >> (int)(operand as IBinaryOperable<BigInteger>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IBinaryOperable Equal(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => BooleanWrapper.FromBool(Value == (operand as IBinaryOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => BooleanWrapper.FromBool(Value == (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BooleanWrapper.FromBool((uint)Value == (operand as IBinaryOperable<BigDecimal>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IBinaryOperable NotEqual(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => BooleanWrapper.FromBool(Value != (operand as IBinaryOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => BooleanWrapper.FromBool(Value != (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BooleanWrapper.FromBool((uint)Value != (operand as IBinaryOperable<BigDecimal>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IBinaryOperable GreaterThan(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => BooleanWrapper.FromBool(Value > (operand as IBinaryOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => BooleanWrapper.FromBool(Value > (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BooleanWrapper.FromBool((uint)Value > (operand as IBinaryOperable<BigDecimal>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IBinaryOperable GreaterThanOrEqual(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => BooleanWrapper.FromBool(Value >= (operand as IBinaryOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => BooleanWrapper.FromBool(Value >= (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BooleanWrapper.FromBool((uint)Value >= (operand as IBinaryOperable<BigDecimal>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IBinaryOperable LessThan(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => BooleanWrapper.FromBool(Value < (operand as IBinaryOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => BooleanWrapper.FromBool(Value < (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BooleanWrapper.FromBool((uint)Value < (operand as IBinaryOperable<BigDecimal>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IBinaryOperable LessThanOrEqual(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => BooleanWrapper.FromBool(Value <= (operand as IBinaryOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => BooleanWrapper.FromBool(Value <= (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BooleanWrapper.FromBool((uint)Value <= (operand as IBinaryOperable<BigDecimal>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IBinaryOperable<bool> StrictEqual(IBinaryOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BooleanWrapper.False;

            return BooleanWrapper.FromBool(Value == (byte)operand.Value);
        }

        public override IBinaryOperable<bool> StrictNotEqual(IBinaryOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BooleanWrapper.True;

            return BooleanWrapper.FromBool(Value != (byte)operand.Value);
        }

        public static implicit operator ByteWrapper(byte b) => new ByteWrapper(b);

        public override string ToString() => "b'" + Value.ToString();
    }
}
