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

        public override IOperable Add(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => new ByteWrapper(Value + (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value + (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper((int)Value + (operand as IOperable<BigDecimal>).Value),
                ObjectType.String => new StringWrapper(Value.ToString() + operand.ToString()),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable Subtract(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => new ByteWrapper(Value - (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value - (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper((uint)Value - (operand as IOperable<BigDecimal>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable Multiply(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => new ByteWrapper(Value * (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value * (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper((uint)Value * (operand as IOperable<BigDecimal>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable Divide(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => new ByteWrapper(Value / (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value / (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper((uint)Value / (operand as IOperable<BigDecimal>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable Mod(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => new ByteWrapper(Value % (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value % (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper((uint)Value % (operand as IOperable<BigDecimal>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable BitwiseAnd(Func<IOperable> operand)
        {
            IOperable eval = operand();

            return eval.OperableType switch
            {
                ObjectType.UnsignedByte => new ByteWrapper(Value & (eval as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable BitwiseOr(Func<IOperable> operand)
        {
            IOperable eval = operand();

            return eval.OperableType switch
            {
                ObjectType.UnsignedByte => new ByteWrapper(Value | (eval as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable BitwiseXOr(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => new ByteWrapper(Value ^ (operand as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable ShiftLeft(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => new ByteWrapper(Value << (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => new ByteWrapper(Value << (int)(operand as IOperable<BigInteger>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable ShiftRight(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => new ByteWrapper(Value >> (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => new ByteWrapper(Value >> (int)(operand as IOperable<BigInteger>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable Equal(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => BooleanWrapper.FromBool(Value == (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => BooleanWrapper.FromBool(Value == (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BooleanWrapper.FromBool((uint)Value == (operand as IOperable<BigDecimal>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable NotEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => BooleanWrapper.FromBool(Value != (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => BooleanWrapper.FromBool(Value != (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BooleanWrapper.FromBool((uint)Value != (operand as IOperable<BigDecimal>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable GreaterThan(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => BooleanWrapper.FromBool(Value > (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => BooleanWrapper.FromBool(Value > (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BooleanWrapper.FromBool((uint)Value > (operand as IOperable<BigDecimal>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable GreaterThanOrEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => BooleanWrapper.FromBool(Value >= (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => BooleanWrapper.FromBool(Value >= (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BooleanWrapper.FromBool((uint)Value >= (operand as IOperable<BigDecimal>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable LessThan(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => BooleanWrapper.FromBool(Value < (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => BooleanWrapper.FromBool(Value < (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BooleanWrapper.FromBool((uint)Value < (operand as IOperable<BigDecimal>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable LessThanOrEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.UnsignedByte => BooleanWrapper.FromBool(Value <= (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryBitInteger => BooleanWrapper.FromBool(Value <= (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BooleanWrapper.FromBool((uint)Value <= (operand as IOperable<BigDecimal>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable<bool> StrictEqual(IOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BooleanWrapper.False;

            return BooleanWrapper.FromBool(Value == (byte)operand.Value);
        }

        public override IBinaryOperable<bool> StrictNotEqual(IOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BooleanWrapper.True;

            return BooleanWrapper.FromBool(Value != (byte)operand.Value);
        }

        public static implicit operator ByteWrapper(byte b) => new ByteWrapper(b);

        public override string ToString() => "b'" + Value.ToString();
    }
}
