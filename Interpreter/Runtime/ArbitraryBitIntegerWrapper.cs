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

        public override IBinaryOperable Add(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value + (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper(Value + (operand as IBinaryOperable<BigDecimal>).Value),
                ObjectType.UnsignedByte => new ArbitraryBitIntegerWrapper(Value + (operand as IBinaryOperable<byte>).Value),
                ObjectType.String => new StringWrapper(Value.ToString() + operand.ToString()),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public override IBinaryOperable BitwiseAnd(Func<IBinaryOperable> operand)
        {
            IBinaryOperable eval = operand();

            return eval.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value & (eval as IBinaryOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => new ArbitraryBitIntegerWrapper(Value & (eval as IBinaryOperable<byte>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public override IBinaryOperable BitwiseOr(Func<IBinaryOperable> operand)
        {
            IBinaryOperable eval = operand();

            return eval.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value | (eval as IBinaryOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => new ArbitraryBitIntegerWrapper(Value | (eval as IBinaryOperable<byte>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public override IBinaryOperable BitwiseXOr(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value ^ (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => new ArbitraryBitIntegerWrapper(Value ^ (operand as IBinaryOperable<byte>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public override IBinaryOperable Divide(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value / (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper(Value / (operand as IBinaryOperable<BigDecimal>).Value),
                ObjectType.UnsignedByte => new ArbitraryBitIntegerWrapper(Value / (operand as IBinaryOperable<byte>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public override IBinaryOperable Equal(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => BooleanWrapper.FromBool(Value == (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BooleanWrapper.FromBool(Value == (operand as IBinaryOperable<BigDecimal>).Value),
                ObjectType.UnsignedByte => BooleanWrapper.FromBool(Value == (operand as IBinaryOperable<byte>).Value),
                ObjectType.NullReference => BooleanWrapper.False,
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public override IBinaryOperable GreaterThan(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => BooleanWrapper.FromBool(Value > (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BooleanWrapper.FromBool(Value > (operand as IBinaryOperable<BigDecimal>).Value),
                ObjectType.UnsignedByte => BooleanWrapper.FromBool(Value > (operand as IBinaryOperable<byte>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public override IBinaryOperable GreaterThanOrEqual(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => BooleanWrapper.FromBool(Value >= (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BooleanWrapper.FromBool(Value >= (operand as IBinaryOperable<BigDecimal>).Value),
                ObjectType.UnsignedByte => BooleanWrapper.FromBool(Value >= (operand as IBinaryOperable<BigDecimal>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public override IBinaryOperable LessThan(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => BooleanWrapper.FromBool(Value < (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BooleanWrapper.FromBool(Value < (operand as IBinaryOperable<BigDecimal>).Value),
                ObjectType.UnsignedByte => BooleanWrapper.FromBool(Value < (operand as IBinaryOperable<byte>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public override IBinaryOperable LessThanOrEqual(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => BooleanWrapper.FromBool(Value <= (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BooleanWrapper.FromBool(Value <= (operand as IBinaryOperable<BigDecimal>).Value),
                ObjectType.UnsignedByte => BooleanWrapper.FromBool(Value <= (operand as IBinaryOperable<byte>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public override IBinaryOperable LogicalAnd(Func<IBinaryOperable> operand) => BitwiseAnd(operand);

        public override IBinaryOperable LogicalOr(Func<IBinaryOperable> operand) => BitwiseOr(operand);

        public override IBinaryOperable LogicalXOr(IBinaryOperable operand) => BitwiseXOr(operand);

        public override IBinaryOperable Mod(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value % (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper(Value % (operand as IBinaryOperable<BigDecimal>).Value),
                ObjectType.UnsignedByte => new ArbitraryBitIntegerWrapper(Value % (operand as IBinaryOperable<byte>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public override IBinaryOperable Multiply(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value * (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper(Value * (operand as IBinaryOperable<BigDecimal>).Value),
                ObjectType.UnsignedByte => new ArbitraryBitIntegerWrapper(Value * (operand as IBinaryOperable<byte>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public override IBinaryOperable NotEqual(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => BooleanWrapper.FromBool(Value != (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BooleanWrapper.FromBool(Value != (operand as IBinaryOperable<BigDecimal>).Value),
                ObjectType.UnsignedByte => BooleanWrapper.FromBool(Value != (operand as IBinaryOperable<byte>).Value),
                ObjectType.NullReference => BooleanWrapper.True,
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public override IBinaryOperable ShiftLeft(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value << (int)(operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => new ArbitraryBitIntegerWrapper(Value << (operand as IBinaryOperable<byte>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public override IBinaryOperable ShiftRight(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value >> (int)(operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => new ArbitraryBitIntegerWrapper(Value >> (operand as IBinaryOperable<byte>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public override IBinaryOperable Subtract(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value - (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper(Value - (operand as IBinaryOperable<BigDecimal>).Value),
                ObjectType.UnsignedByte => new ArbitraryBitIntegerWrapper(Value - (operand as IBinaryOperable<byte>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public override IBinaryOperable<bool> StrictEqual(IBinaryOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BooleanWrapper.False;

            return BooleanWrapper.FromBool(Value == (BigInteger)operand.Value);
        }

        public override IBinaryOperable<bool> StrictNotEqual(IBinaryOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BooleanWrapper.True;

            return BooleanWrapper.FromBool(Value != (BigInteger)operand.Value);
        }

        public static implicit operator ArbitraryBitIntegerWrapper(BigInteger value) => new ArbitraryBitIntegerWrapper(value);

        public override string ToString() => Value.ToString();
    }
}
