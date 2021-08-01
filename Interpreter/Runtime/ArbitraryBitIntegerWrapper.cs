using Singulink.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Runtime
{
    public class ArbitraryBitIntegerWrapper : IBinaryOperable<BigInteger> // TODO: Implement ArbitraryBitIntegerWrapper
    {
        object IBinaryOperable.Value { get; set; }
        public BigInteger Value
        {
            get => (BigInteger)(this as IBinaryOperable).Value;
            set => (this as IBinaryOperable).Value = value;
        }

        public ObjectType OperableType => ObjectType.ArbitraryBitInteger;

        public ArbitraryBitIntegerWrapper(BigInteger value) => Value = value;

        public IBinaryOperable Add(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value + (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper(Value + (operand as IBinaryOperable<BigDecimal>).Value),
                ObjectType.String => new StringWrapper(Value.ToString() + operand.ToString()),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public IBinaryOperable BitwiseAnd(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value & (operand as IBinaryOperable<BigInteger>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public IBinaryOperable BitwiseOr(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value | (operand as IBinaryOperable<BigInteger>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public IBinaryOperable BitwiseXOr(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value ^ (operand as IBinaryOperable<BigInteger>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public IBinaryOperable Divide(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value / (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper(Value / (operand as IBinaryOperable<BigDecimal>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public IBinaryOperable Equal(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new BooleanWrapper(Value == (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new BooleanWrapper(Value == (operand as IBinaryOperable<BigDecimal>).Value),
                _ => new BooleanWrapper(false)
            };
        }

        public IBinaryOperable GreaterThan(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new BooleanWrapper(Value > (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new BooleanWrapper(Value > (operand as IBinaryOperable<BigDecimal>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public IBinaryOperable GreaterThanOrEqual(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new BooleanWrapper(Value >= (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new BooleanWrapper(Value >= (operand as IBinaryOperable<BigDecimal>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public IBinaryOperable LessThan(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new BooleanWrapper(Value < (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new BooleanWrapper(Value < (operand as IBinaryOperable<BigDecimal>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public IBinaryOperable LessThanOrEqual(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new BooleanWrapper(Value <= (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new BooleanWrapper(Value <= (operand as IBinaryOperable<BigDecimal>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public IBinaryOperable LogicalAnd(IBinaryOperable operand) => BitwiseAnd(operand);

        public IBinaryOperable LogicalOr(IBinaryOperable operand) => BitwiseOr(operand);

        public IBinaryOperable LogicalXOr(IBinaryOperable operand) => BitwiseXOr(operand);

        public IBinaryOperable Mod(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value % (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper(Value % (operand as IBinaryOperable<BigDecimal>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public IBinaryOperable Multiply(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value * (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper(Value * (operand as IBinaryOperable<BigDecimal>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public IBinaryOperable NotEqual(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new BooleanWrapper(Value != (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new BooleanWrapper(Value == (operand as IBinaryOperable<BigDecimal>).Value),
                _ => new BooleanWrapper(true)
            };
        }

        public IBinaryOperable ShiftLeft(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value << (int)(operand as IBinaryOperable<BigInteger>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public IBinaryOperable ShiftRight(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value >> (int)(operand as IBinaryOperable<BigInteger>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public IBinaryOperable Subtract(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new ArbitraryBitIntegerWrapper(Value - (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper(Value - (operand as IBinaryOperable<BigDecimal>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public override string ToString() => Value.ToString();
    }
}
