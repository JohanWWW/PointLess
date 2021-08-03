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
    public class ArbitraryPrecisionDecimalWrapper : WrapperBase<BigDecimal>
    {
        private const string DECIMAL_FORMAT = "G"; // General

        private static readonly System.Globalization.CultureInfo DECIMAL_CULTURE = 
            System.Globalization.CultureInfo.InvariantCulture;

        public ArbitraryPrecisionDecimalWrapper(BigDecimal value) : base(value, ObjectType.ArbitraryPrecisionDecimal)
        {
        }

        public override IBinaryOperable Add(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper(Value + (operand as IBinaryOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => new ArbitraryPrecisionDecimalWrapper(Value + (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.String => new StringWrapper(ToString() + operand.ToString()),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public override IBinaryOperable Divide(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper(Value / (operand as IBinaryOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => new ArbitraryPrecisionDecimalWrapper(Value / (operand as IBinaryOperable<BigInteger>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public override IBinaryOperable Equal(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => BooleanWrapper.FromBool(Value == (operand as IBinaryOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => BooleanWrapper.FromBool(Value == (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.NullReference => BooleanWrapper.False,
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public override IBinaryOperable GreaterThan(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => BooleanWrapper.FromBool(Value > (operand as IBinaryOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => BooleanWrapper.FromBool(Value > (operand as IBinaryOperable<BigInteger>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public override IBinaryOperable GreaterThanOrEqual(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => BooleanWrapper.FromBool(Value >= (operand as IBinaryOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => BooleanWrapper.FromBool(Value >= (operand as IBinaryOperable<BigInteger>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public override IBinaryOperable LessThan(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => BooleanWrapper.FromBool(Value < (operand as IBinaryOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => BooleanWrapper.FromBool(Value < (operand as IBinaryOperable<BigInteger>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public override IBinaryOperable LessThanOrEqual(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => BooleanWrapper.FromBool(Value <= (operand as IBinaryOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => BooleanWrapper.FromBool(Value <= (operand as IBinaryOperable<BigInteger>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public override IBinaryOperable Mod(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper(Value % (operand as IBinaryOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => new ArbitraryPrecisionDecimalWrapper(Value % (operand as IBinaryOperable<BigInteger>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public override IBinaryOperable Multiply(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper(Value * (operand as IBinaryOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => new ArbitraryPrecisionDecimalWrapper(Value * (operand as IBinaryOperable<BigInteger>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public override IBinaryOperable NotEqual(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => BooleanWrapper.FromBool(Value != (operand as IBinaryOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => BooleanWrapper.FromBool(Value != (operand as IBinaryOperable<BigInteger>).Value),
                ObjectType.NullReference => BooleanWrapper.True,
                //_ => new BooleanWrapper(true)
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public override IBinaryOperable Subtract(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper(Value - (operand as IBinaryOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => new ArbitraryPrecisionDecimalWrapper(Value - (operand as IBinaryOperable<BigInteger>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public override IBinaryOperable<bool> StrictEqual(IBinaryOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BooleanWrapper.False;

            return BooleanWrapper.FromBool(Value == (BigDecimal)operand.Value);
        }

        public override IBinaryOperable<bool> StrictNotEqual(IBinaryOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BooleanWrapper.True;

            return BooleanWrapper.FromBool(Value != (BigDecimal)operand.Value);
        }

        public override string ToString() => Value.ToString(DECIMAL_FORMAT, DECIMAL_CULTURE);
    }
}
