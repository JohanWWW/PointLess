using Singulink.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Runtime
{
    public class ArbitraryPrecisionDecimalWrapper : IBinaryOperable<BigDecimal> // TODO: Implement ArbitraryPrecisionDecimalWrapper
    {
        public BigDecimal Value
        {
            get => (BigDecimal)(this as IBinaryOperable).Value;
            set => (this as IBinaryOperable).Value = value;
        }

        public ObjectType OperableType => ObjectType.ArbitraryPrecisionDecimal;

        object IBinaryOperable.Value { get; set; }

        public ArbitraryPrecisionDecimalWrapper(BigDecimal value) => Value = value;

        public IBinaryOperable Add(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper(Value + (operand as IBinaryOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => new ArbitraryPrecisionDecimalWrapper(Value + (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.String => new StringWrapper(Value.ToString() + operand.Value.ToString()),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public IBinaryOperable BitwiseAnd(IBinaryOperable operand)
        {
            throw new MissingBinaryOperatorOverrideException();
        }

        public IBinaryOperable BitwiseOr(IBinaryOperable operand)
        {
            throw new MissingBinaryOperatorOverrideException();

        }

        public IBinaryOperable BitwiseXOr(IBinaryOperable operand)
        {
            throw new MissingBinaryOperatorOverrideException();

        }

        public IBinaryOperable Divide(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper(Value / (operand as IBinaryOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => new ArbitraryPrecisionDecimalWrapper(Value / (operand as IBinaryOperable<BigInteger>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public IBinaryOperable Equal(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => new BooleanWrapper(Value == (operand as IBinaryOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => new BooleanWrapper(Value == (operand as IBinaryOperable<BigInteger>).Value),
                _ => new BooleanWrapper(false)
            };
        }

        public IBinaryOperable GreaterThan(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => new BooleanWrapper(Value > (operand as IBinaryOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => new BooleanWrapper(Value > (operand as IBinaryOperable<BigInteger>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public IBinaryOperable GreaterThanOrEqual(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => new BooleanWrapper(Value >= (operand as IBinaryOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => new BooleanWrapper(Value >= (operand as IBinaryOperable<BigInteger>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public IBinaryOperable LessThan(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => new BooleanWrapper(Value < (operand as IBinaryOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => new BooleanWrapper(Value < (operand as IBinaryOperable<BigInteger>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public IBinaryOperable LessThanOrEqual(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => new BooleanWrapper(Value <= (operand as IBinaryOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => new BooleanWrapper(Value <= (operand as IBinaryOperable<BigInteger>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public IBinaryOperable LogicalAnd(IBinaryOperable operand)
        {
            throw new MissingBinaryOperatorOverrideException();

        }

        public IBinaryOperable LogicalOr(IBinaryOperable operand)
        {
            throw new MissingBinaryOperatorOverrideException();

        }

        public IBinaryOperable LogicalXOr(IBinaryOperable operand)
        {
            throw new MissingBinaryOperatorOverrideException();

        }

        public IBinaryOperable Mod(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper(Value % (operand as IBinaryOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => new ArbitraryPrecisionDecimalWrapper(Value % (operand as IBinaryOperable<BigInteger>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public IBinaryOperable Multiply(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper(Value * (operand as IBinaryOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => new ArbitraryPrecisionDecimalWrapper(Value * (operand as IBinaryOperable<BigInteger>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public IBinaryOperable NotEqual(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => new BooleanWrapper(Value != (operand as IBinaryOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => new BooleanWrapper(Value != (operand as IBinaryOperable<BigInteger>).Value),
                _ => new BooleanWrapper(true)
            };
        }

        public IBinaryOperable ShiftLeft(IBinaryOperable operand)
        {
            throw new MissingBinaryOperatorOverrideException();

        }

        public IBinaryOperable ShiftRight(IBinaryOperable operand)
        {
            throw new MissingBinaryOperatorOverrideException();

        }

        public IBinaryOperable Subtract(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper(Value - (operand as IBinaryOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => new ArbitraryPrecisionDecimalWrapper(Value - (operand as IBinaryOperable<BigInteger>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public override string ToString() => Value.ToString();
    }
}
