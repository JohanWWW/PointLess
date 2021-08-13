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
    public class BigIntOperable : OperableBase<BigInteger>
    {
        public BigIntOperable(BigInteger value) : base(value, ObjectType.ArbitraryBitInteger)
        {
        }

        #region Binary Operators

        public override IOperable Add(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new BigIntOperable(Value + (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new BigDecimalOperable(Value + (operand as IOperable<BigDecimal>).Value),
                ObjectType.UnsignedByte => new BigIntOperable(Value + (operand as IOperable<byte>).Value),
                ObjectType.String => new StringOperable(Value.ToString() + operand.ToString()),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable BitwiseAnd(Func<IOperable> operand)
        {
            IOperable eval = operand();

            return eval.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new BigIntOperable(Value & (eval as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => new BigIntOperable(Value & (eval as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable BitwiseOr(Func<IOperable> operand)
        {
            IOperable eval = operand();

            return eval.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new BigIntOperable(Value | (eval as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => new BigIntOperable(Value | (eval as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable BitwiseXOr(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new BigIntOperable(Value ^ (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => new BigIntOperable(Value ^ (operand as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable Divide(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new BigIntOperable(Value / (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new BigDecimalOperable(Value / (operand as IOperable<BigDecimal>).Value),
                ObjectType.UnsignedByte => new BigIntOperable(Value / (operand as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable Equal(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value == (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool(Value == (operand as IOperable<BigDecimal>).Value),
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value == (operand as IOperable<byte>).Value),
                ObjectType.Void => BoolOperable.False,
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable GreaterThan(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value > (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool(Value > (operand as IOperable<BigDecimal>).Value),
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value > (operand as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable GreaterThanOrEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value >= (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool(Value >= (operand as IOperable<BigDecimal>).Value),
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value >= (operand as IOperable<BigDecimal>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable LessThan(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value < (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool(Value < (operand as IOperable<BigDecimal>).Value),
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value < (operand as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable LessThanOrEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value <= (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool(Value <= (operand as IOperable<BigDecimal>).Value),
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value <= (operand as IOperable<byte>).Value),
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
                ObjectType.ArbitraryBitInteger => new BigIntOperable(Value % (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new BigDecimalOperable(Value % (operand as IOperable<BigDecimal>).Value),
                ObjectType.UnsignedByte => new BigIntOperable(Value % (operand as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable Multiply(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new BigIntOperable(Value * (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new BigDecimalOperable(Value * (operand as IOperable<BigDecimal>).Value),
                ObjectType.UnsignedByte => new BigIntOperable(Value * (operand as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable NotEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value != (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool(Value != (operand as IOperable<BigDecimal>).Value),
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value != (operand as IOperable<byte>).Value),
                ObjectType.Void => BoolOperable.True,
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable ShiftLeft(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new BigIntOperable(Value << (int)(operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => new BigIntOperable(Value << (operand as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable ShiftRight(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new BigIntOperable(Value >> (int)(operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => new BigIntOperable(Value >> (operand as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable Subtract(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new BigIntOperable(Value - (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new BigDecimalOperable(Value - (operand as IOperable<BigDecimal>).Value),
                ObjectType.UnsignedByte => new BigIntOperable(Value - (operand as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable<bool> StrictEqual(IOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BoolOperable.False;

            return BoolOperable.FromBool(Value == (BigInteger)operand.Value);
        }

        public override IOperable<bool> StrictNotEqual(IOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BoolOperable.True;

            return BoolOperable.FromBool(Value != (BigInteger)operand.Value);
        }

        #endregion

        #region Unary Operators

        public override IOperable UnaryMinus() => (BigIntOperable)BigInteger.Negate(Value);

        #endregion

        public static implicit operator BigIntOperable(BigInteger value) => new BigIntOperable(value);

        public override string ToString() => Value.ToString();
    }
}
