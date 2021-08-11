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

        public override IOperable Add(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper(Value + (operand as IOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => new ArbitraryPrecisionDecimalWrapper(Value + (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => new ArbitraryPrecisionDecimalWrapper(Value + (uint)(operand as IOperable<byte>).Value),
                ObjectType.String => new StringWrapper(ToString() + operand.ToString()),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable Divide(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper(Value / (operand as IOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => new ArbitraryPrecisionDecimalWrapper(Value / (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => new ArbitraryPrecisionDecimalWrapper(Value / (uint)(operand as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable Equal(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => BooleanWrapper.FromBool(Value == (operand as IOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => BooleanWrapper.FromBool(Value == (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => BooleanWrapper.FromBool(Value == (uint)(operand as IOperable<byte>).Value),
                ObjectType.NullReference => BooleanWrapper.False,
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable GreaterThan(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => BooleanWrapper.FromBool(Value > (operand as IOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => BooleanWrapper.FromBool(Value > (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => BooleanWrapper.FromBool(Value > (uint)(operand as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable GreaterThanOrEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => BooleanWrapper.FromBool(Value >= (operand as IOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => BooleanWrapper.FromBool(Value >= (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => BooleanWrapper.FromBool(Value >= (uint)(operand as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable LessThan(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => BooleanWrapper.FromBool(Value < (operand as IOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => BooleanWrapper.FromBool(Value < (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => BooleanWrapper.FromBool(Value < (uint)(operand as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable LessThanOrEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => BooleanWrapper.FromBool(Value <= (operand as IOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => BooleanWrapper.FromBool(Value <= (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => BooleanWrapper.FromBool(Value <= (uint)(operand as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable Mod(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper(Value % (operand as IOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => new ArbitraryPrecisionDecimalWrapper(Value % (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => new ArbitraryPrecisionDecimalWrapper(Value % (uint)(operand as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable Multiply(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper(Value * (operand as IOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => new ArbitraryPrecisionDecimalWrapper(Value * (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => new ArbitraryPrecisionDecimalWrapper(Value * (uint)(operand as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable NotEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => BooleanWrapper.FromBool(Value != (operand as IOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => BooleanWrapper.FromBool(Value != (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => BooleanWrapper.FromBool(Value != (uint)(operand as IOperable<byte>).Value),
                ObjectType.NullReference => BooleanWrapper.True,
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable Subtract(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryPrecisionDecimal => new ArbitraryPrecisionDecimalWrapper(Value - (operand as IOperable<BigDecimal>).Value),
                ObjectType.ArbitraryBitInteger => new ArbitraryPrecisionDecimalWrapper(Value - (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => new ArbitraryPrecisionDecimalWrapper(Value - (uint)(operand as IOperable<byte>).Value),
                _ => throw new MissingOperatorOverrideException()
            };
        }

        public override IOperable<bool> StrictEqual(IOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BooleanWrapper.False;

            return BooleanWrapper.FromBool(Value == (BigDecimal)operand.Value);
        }

        public override IOperable<bool> StrictNotEqual(IOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BooleanWrapper.True;

            return BooleanWrapper.FromBool(Value != (BigDecimal)operand.Value);
        }

        public override string ToString() => Value.ToString(DECIMAL_FORMAT, DECIMAL_CULTURE);
    }
}
