using Interpreter.Models.Enums;
using Interpreter.Types;
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
                ObjectType.ArbitraryBitInteger => (BigIntOperable)(Value + (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => (BigDecimalOperable)(Value + (operand as IOperable<BigDecimal>).Value),
                ObjectType.UnsignedByte => (BigIntOperable)(Value + (operand as IOperable<byte>).Value),
                ObjectType.String => (StringOperable)(Value.ToString() + operand.ToString()),
                ObjectType.StringObject => (StringObjectOperable)(Value.ToString() + operand.ToString()),
                ObjectType.Utf32Character => (BigIntOperable)(Value + (operand as IOperable<Utf32Character>).Value.Value),
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Add)
            };
        }

        public override IOperable BitwiseAnd(Func<IOperable> operand)
        {
            IOperable eval = operand();

            return eval.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => (BigIntOperable)(Value & (eval as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => Value & (eval as IOperable<byte>).Value,
                ObjectType.Utf32Character => Value & (eval as IOperable<Utf32Character>).Value.Value,
                _ => throw MissingBinaryOperatorImplementation(eval, BinaryOperator.BitwiseAnd)
            };
        }

        public override IOperable BitwiseOr(Func<IOperable> operand)
        {
            IOperable eval = operand();

            return eval.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => (BigIntOperable)(Value | (eval as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => Value | (eval as IOperable<byte>).Value,
                ObjectType.Utf32Character => Value | (eval as IOperable<Utf32Character>).Value.Value,
                _ => throw MissingBinaryOperatorImplementation(eval, BinaryOperator.BitwiseOr)
            };
        }

        public override IOperable BitwiseXOr(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => (BigIntOperable)(Value ^ (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => Value ^ (operand as IOperable<byte>).Value,
                ObjectType.Utf32Character => Value ^ (operand as IOperable<Utf32Character>).Value.Value,
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.BitwiseXOr)
            };
        }

        public override IOperable Divide(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => (BigIntOperable)(Value / (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => (BigDecimalOperable)(Value / (operand as IOperable<BigDecimal>).Value),
                ObjectType.Utf32Character => (BigIntOperable)(Value / (operand as IOperable<Utf32Character>).Value.Value),
                ObjectType.UnsignedByte => (BigIntOperable)(Value / (operand as IOperable<byte>).Value),
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Div)
            };
        }

        public override IOperable Equal(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value == (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool(Value == (operand as IOperable<BigDecimal>).Value),
                ObjectType.Utf32Character => BoolOperable.FromBool(Value == (operand as IOperable<Utf32Character>).Value.Value),
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value == (operand as IOperable<byte>).Value),
                ObjectType.Void => BoolOperable.False,
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Equal)
            };
        }

        public override IOperable GreaterThan(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value > (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool(Value > (operand as IOperable<BigDecimal>).Value),
                ObjectType.Utf32Character => BoolOperable.FromBool(Value > (operand as IOperable<Utf32Character>).Value.Value),
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value > (operand as IOperable<byte>).Value),
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.GreaterThan)
            };
        }

        public override IOperable GreaterThanOrEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value >= (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool(Value >= (operand as IOperable<BigDecimal>).Value),
                ObjectType.Utf32Character => BoolOperable.FromBool(Value >= (operand as IOperable<Utf32Character>).Value.Value),
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value >= (operand as IOperable<BigDecimal>).Value),
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.GreaterThanOrEqual)
            };
        }

        public override IOperable LessThan(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value < (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool(Value < (operand as IOperable<BigDecimal>).Value),
                ObjectType.Utf32Character => BoolOperable.FromBool(Value < (operand as IOperable<Utf32Character>).Value.Value),
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value < (operand as IOperable<byte>).Value),
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.LessThan)
            };
        }

        public override IOperable LessThanOrEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value <= (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool(Value <= (operand as IOperable<BigDecimal>).Value),
                ObjectType.Utf32Character => BoolOperable.FromBool(Value <= (operand as IOperable<Utf32Character>).Value.Value),
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value <= (operand as IOperable<byte>).Value),
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.LessThanOrEqual)
            };
        }

        public override IOperable LogicalAnd(Func<IOperable> operand) => BitwiseAnd(operand);

        public override IOperable LogicalOr(Func<IOperable> operand) => BitwiseOr(operand);

        public override IOperable LogicalXOr(IOperable operand) => BitwiseXOr(operand);

        public override IOperable Mod(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => (BigIntOperable)(Value % (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => (BigDecimalOperable)(Value % (operand as IOperable<BigDecimal>).Value),
                ObjectType.Utf32Character => (BigIntOperable)(Value % (operand as IOperable<Utf32Character>).Value.Value),
                ObjectType.UnsignedByte => (BigIntOperable)(Value % (operand as IOperable<byte>).Value),
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Mod)
            };
        }

        public override IOperable Multiply(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => new BigIntOperable(Value * (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => (BigDecimalOperable)(Value * (operand as IOperable<BigDecimal>).Value),
                ObjectType.Utf32Character => (BigIntOperable)(Value * (operand as IOperable<Utf32Character>).Value.Value),
                ObjectType.UnsignedByte => (BigIntOperable)(Value * (operand as IOperable<byte>).Value),
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Mult)
            };
        }

        public override IOperable NotEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value != (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool(Value != (operand as IOperable<BigDecimal>).Value),
                ObjectType.Utf32Character => BoolOperable.FromBool(Value != (operand as IOperable<Utf32Character>).Value.Value),
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value != (operand as IOperable<byte>).Value),
                ObjectType.Void => BoolOperable.True,
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.NotEqual)
            };
        }

        public override IOperable ShiftLeft(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => (BigIntOperable)(Value << (int)(operand as IOperable<BigInteger>).Value),
                ObjectType.Utf32Character => Value << (operand as IOperable<Utf32Character>).Value.Value,
                ObjectType.UnsignedByte => Value << (operand as IOperable<byte>).Value,
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.ShiftLeft)
            };
        }

        public override IOperable ShiftRight(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => (BigIntOperable)(Value >> (int)(operand as IOperable<BigInteger>).Value),
                ObjectType.Utf32Character => Value >> (operand as IOperable<Utf32Character>).Value.Value,
                ObjectType.UnsignedByte => Value >> (operand as IOperable<byte>).Value,
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.ShiftRight)
            };
        }

        public override IOperable Subtract(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.ArbitraryBitInteger => (BigIntOperable)(Value - (operand as IOperable<BigInteger>).Value),
                ObjectType.ArbitraryPrecisionDecimal => (BigDecimalOperable)(Value - (operand as IOperable<BigDecimal>).Value),
                ObjectType.Utf32Character => (BigIntOperable)(Value - (operand as IOperable<Utf32Character>).Value.Value),
                ObjectType.UnsignedByte => (BigIntOperable)(Value - (operand as IOperable<byte>).Value),
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Sub)
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

        public static implicit operator BigIntOperable(BigInteger value) => new(value);
        public static implicit operator BigIntOperable(int value) => new(value);

        public override string ToString() => Value.ToString();
    }
}
