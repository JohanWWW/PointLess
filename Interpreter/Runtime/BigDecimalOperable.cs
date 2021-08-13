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
    public class BigDecimalOperable : OperableBase<BigDecimal>
    {
        private const string DECIMAL_FORMAT = "G"; // General

        private static readonly System.Globalization.CultureInfo DECIMAL_CULTURE = 
            System.Globalization.CultureInfo.InvariantCulture;

        public BigDecimalOperable(BigDecimal value) : base(value, ObjectType.ArbitraryPrecisionDecimal)
        {
        }

        #region Binary Operator

        public override IOperable Add(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => new BigDecimalOperable(Value + (operand as IOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => new BigDecimalOperable(Value + (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => new BigDecimalOperable(Value + (uint)(operand as IOperable<byte>).Value),
                ObjectType.String => new StringOperable(ToString() + operand.ToString()),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable Divide(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => new BigDecimalOperable(Value / (operand as IOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => new BigDecimalOperable(Value / (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => new BigDecimalOperable(Value / (uint)(operand as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable Equal(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool(Value == (operand as IOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value == (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value == (uint)(operand as IOperable<byte>).Value),
                ObjectType.Void => BoolOperable.False,
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable GreaterThan(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool(Value > (operand as IOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value > (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value > (uint)(operand as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable GreaterThanOrEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool(Value >= (operand as IOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value >= (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value >= (uint)(operand as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable LessThan(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool(Value < (operand as IOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value < (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value < (uint)(operand as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable LessThanOrEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool(Value <= (operand as IOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value <= (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value <= (uint)(operand as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable Mod(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => new BigDecimalOperable(Value % (operand as IOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => new BigDecimalOperable(Value % (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => new BigDecimalOperable(Value % (uint)(operand as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable Multiply(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => new BigDecimalOperable(Value * (operand as IOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => new BigDecimalOperable(Value * (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => new BigDecimalOperable(Value * (uint)(operand as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable NotEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool(Value != (operand as IOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value != (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value != (uint)(operand as IOperable<byte>).Value),
                ObjectType.Void => BoolOperable.True,
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable Subtract(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => new BigDecimalOperable(Value - (operand as IOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => new BigDecimalOperable(Value - (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => new BigDecimalOperable(Value - (uint)(operand as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable<bool> StrictEqual(IOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BoolOperable.False;

            return BoolOperable.FromBool(Value == (BigDecimal)operand.Value);
        }

        public override IOperable<bool> StrictNotEqual(IOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BoolOperable.True;

            return BoolOperable.FromBool(Value != (BigDecimal)operand.Value);
        }

        #endregion

        #region Unary Operator

        public override IOperable UnaryMinus() => (BigDecimalOperable)(-Value);

        #endregion

        public static implicit operator BigDecimalOperable(BigDecimal value) => new BigDecimalOperable(value);

        public override string ToString() => Value.ToString(DECIMAL_FORMAT, DECIMAL_CULTURE);
    }
}
