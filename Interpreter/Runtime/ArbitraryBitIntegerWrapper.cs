using Interpreter.Models.Enums;
using Singulink.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Runtime
{
    public class ArbitraryBitIntegerWrapper : WrapperBase<BigInteger>
    {
        public ArbitraryBitIntegerWrapper(BigInteger value) : base(value, ObjectType.ArbitraryBitInteger)
        {
        }

        public override IOperable Add(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value + (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper(Value + (operand as IOperable<BigDecimal>).Value),
                ObjectType.UnsignedByte => new ArbitraryBitIntegerWrapper(Value + (operand as IOperable<byte>).Value),
                ObjectType.String => new StringWrapper(Value.ToString() + operand.ToString()),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable BitwiseAnd(Func<IOperable> operand)
        {
            IOperable eval = operand();

            return eval.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value & (eval as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => new ArbitraryBitIntegerWrapper(Value & (eval as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable BitwiseOr(Func<IOperable> operand)
        {
            IOperable eval = operand();

            return eval.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value | (eval as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => new ArbitraryBitIntegerWrapper(Value | (eval as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable BitwiseXOr(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value ^ (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => new ArbitraryBitIntegerWrapper(Value ^ (operand as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable Divide(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value / (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper(Value / (operand as IOperable<BigDecimal>).Value),
                ObjectType.UnsignedByte => new ArbitraryBitIntegerWrapper(Value / (operand as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable Equal(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => BooleanWrapper.FromBool(Value == (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BooleanWrapper.FromBool(Value == (operand as IOperable<BigDecimal>).Value),
                ObjectType.UnsignedByte => BooleanWrapper.FromBool(Value == (operand as IOperable<byte>).Value),
                ObjectType.NullReference => BooleanWrapper.False,
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable GreaterThan(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => BooleanWrapper.FromBool(Value > (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BooleanWrapper.FromBool(Value > (operand as IOperable<BigDecimal>).Value),
                ObjectType.UnsignedByte => BooleanWrapper.FromBool(Value > (operand as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable GreaterThanOrEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => BooleanWrapper.FromBool(Value >= (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BooleanWrapper.FromBool(Value >= (operand as IOperable<BigDecimal>).Value),
                ObjectType.UnsignedByte => BooleanWrapper.FromBool(Value >= (operand as IOperable<BigDecimal>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable LessThan(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => BooleanWrapper.FromBool(Value < (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BooleanWrapper.FromBool(Value < (operand as IOperable<BigDecimal>).Value),
                ObjectType.UnsignedByte => BooleanWrapper.FromBool(Value < (operand as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable LessThanOrEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => BooleanWrapper.FromBool(Value <= (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BooleanWrapper.FromBool(Value <= (operand as IOperable<BigDecimal>).Value),
                ObjectType.UnsignedByte => BooleanWrapper.FromBool(Value <= (operand as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable LogicalAnd(Func<IOperable> operand) => BitwiseAnd(operand);

        public override IOperable LogicalOr(Func<IOperable> operand) => BitwiseOr(operand);

        public override IOperable LogicalXOr(IOperable operand) => BitwiseXOr(operand);

        public override IOperable Mod(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value % (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper(Value % (operand as IOperable<BigDecimal>).Value),
                ObjectType.UnsignedByte => new ArbitraryBitIntegerWrapper(Value % (operand as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable Multiply(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value * (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper(Value * (operand as IOperable<BigDecimal>).Value),
                ObjectType.UnsignedByte => new ArbitraryBitIntegerWrapper(Value * (operand as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable NotEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => BooleanWrapper.FromBool(Value != (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BooleanWrapper.FromBool(Value != (operand as IOperable<BigDecimal>).Value),
                ObjectType.UnsignedByte => BooleanWrapper.FromBool(Value != (operand as IOperable<byte>).Value),
                ObjectType.NullReference => BooleanWrapper.True,
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable ShiftLeft(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value << (int)(operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => new ArbitraryBitIntegerWrapper(Value << (operand as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable ShiftRight(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value >> (int)(operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => new ArbitraryBitIntegerWrapper(Value >> (operand as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable Subtract(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value - (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper(Value - (operand as IOperable<BigDecimal>).Value),
                ObjectType.UnsignedByte => new ArbitraryBitIntegerWrapper(Value - (operand as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable<bool> StrictEqual(IOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BooleanWrapper.False;

            return BooleanWrapper.FromBool(Value == (BigInteger)operand.Value);
        }

        public override IOperable<bool> StrictNotEqual(IOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BooleanWrapper.True;

            return BooleanWrapper.FromBool(Value != (BigInteger)operand.Value);
        }

        public static implicit operator ArbitraryBitIntegerWrapper(BigInteger value) => new ArbitraryBitIntegerWrapper(value);

        public override string ToString() => Value.ToString();
    }
}
