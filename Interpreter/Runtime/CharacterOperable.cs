using Interpreter.Runtime.Extensions;
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
    public class CharacterOperable : OperableBase<Utf32Character>
    {
        public CharacterOperable(Utf32Character value) : base(value, ObjectType.Utf32Character)
        {
        }

        public CharacterOperable(int value) : this((Utf32Character)value)
        {
        }

        #region Binary Operator Overrides

        public override IOperable Add(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.Utf32Character => (CharacterOperable)(Value.Value + (operand as CharacterOperable).Value.Value),
                ObjectType.ArbitraryBitInteger => (BigIntOperable)(Value.Value + (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => (BigIntOperable)(Value.Value + (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryPrecisionDecimal => (BigDecimalOperable)(Value.Value + (operand as IOperable<BigDecimal>).Value),
                ObjectType.StringObject => (StringObjectOperable)(Value.ToCharString() + (operand as StringObjectOperable).ToString()),
                ObjectType.String => (StringOperable)(Value.ToCharString() + (operand as IOperable<string>).Value),
                _ => throw MissingBinaryOperatorImplementation(operand, Models.Enums.BinaryOperator.Add)
            };
        }

        public override IOperable Subtract(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.Utf32Character => (CharacterOperable)(Value.Value + (operand as CharacterOperable).Value.Value),
                ObjectType.ArbitraryBitInteger => (BigIntOperable)(Value.Value + (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => (BigIntOperable)(Value.Value + (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryPrecisionDecimal => (BigDecimalOperable)(Value.Value + (operand as IOperable<BigDecimal>).Value),
                _ => throw MissingBinaryOperatorImplementation(operand, Models.Enums.BinaryOperator.Sub)
            };
        }

        public override IOperable Multiply(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.Utf32Character => (CharacterOperable)(Value.Value * (operand as CharacterOperable).Value.Value),
                ObjectType.ArbitraryBitInteger => (BigIntOperable)(Value.Value * (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => (BigIntOperable)(Value.Value * (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryPrecisionDecimal => (BigDecimalOperable)(Value.Value * (operand as IOperable<BigDecimal>).Value),
                _ => throw MissingBinaryOperatorImplementation(operand, Models.Enums.BinaryOperator.Mult)
            };
        }

        public override IOperable Divide(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.Utf32Character => (CharacterOperable)(Value.Value / (operand as CharacterOperable).Value.Value),
                ObjectType.ArbitraryBitInteger => (BigIntOperable)(Value.Value / (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => (BigIntOperable)(Value.Value / (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryPrecisionDecimal => (BigDecimalOperable)(Value.Value / (operand as IOperable<BigDecimal>).Value),
                _ => throw MissingBinaryOperatorImplementation(operand, Models.Enums.BinaryOperator.Div)
            };
        }

        public override IOperable Mod(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.Utf32Character => (CharacterOperable)(Value.Value % (operand as CharacterOperable).Value.Value),
                ObjectType.ArbitraryBitInteger => (BigIntOperable)(Value.Value % (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => (BigIntOperable)(Value.Value % (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryPrecisionDecimal => (BigDecimalOperable)(Value.Value % (operand as IOperable<BigDecimal>).Value),
                _ => throw MissingBinaryOperatorImplementation(operand, Models.Enums.BinaryOperator.Mod)
            };
        }

        public override IOperable LessThan(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.Utf32Character => BoolOperable.FromBool(Value.Value < (operand as CharacterOperable).Value.Value),
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value.Value < (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value.Value < (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool(Value.Value < (operand as IOperable<BigDecimal>).Value),
                _ => throw MissingBinaryOperatorImplementation(operand, Models.Enums.BinaryOperator.LessThan)
            };
        }

        public override IOperable LessThanOrEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.Utf32Character => BoolOperable.FromBool(Value.Value <= (operand as CharacterOperable).Value.Value),
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value.Value <= (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value.Value <= (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool(Value.Value <= (operand as IOperable<BigDecimal>).Value),
                _ => throw MissingBinaryOperatorImplementation(operand, Models.Enums.BinaryOperator.LessThanOrEqual)
            };
        }

        public override IOperable GreaterThan(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.Utf32Character => BoolOperable.FromBool(Value.Value > (operand as CharacterOperable).Value.Value),
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value.Value > (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value.Value > (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool(Value.Value > (operand as IOperable<BigDecimal>).Value),
                _ => throw MissingBinaryOperatorImplementation(operand, Models.Enums.BinaryOperator.GreaterThan)
            };
        }

        public override IOperable GreaterThanOrEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.Utf32Character => BoolOperable.FromBool(Value.Value >= (operand as CharacterOperable).Value.Value),
                ObjectType.ArbitraryBitInteger => BoolOperable.FromBool(Value.Value >= (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => BoolOperable.FromBool(Value.Value >= (operand as IOperable<byte>).Value),
                ObjectType.ArbitraryPrecisionDecimal => BoolOperable.FromBool(Value.Value >= (operand as IOperable<BigDecimal>).Value),
                _ => throw MissingBinaryOperatorImplementation(operand, Models.Enums.BinaryOperator.GreaterThanOrEqual)
            };
        }

        public override IOperable BitwiseAnd(Func<IOperable> operand)
        {
            IOperable eval = operand();
            return eval.OperableType switch
            {
                ObjectType.Utf32Character => (CharacterOperable)(Value.Value & (eval as CharacterOperable).Value.Value),
                ObjectType.ArbitraryBitInteger => (BigIntOperable)(Value.Value & (eval as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => (BigIntOperable)(Value.Value & (eval as IOperable<byte>).Value),
                _ => throw MissingBinaryOperatorImplementation(eval, Models.Enums.BinaryOperator.BitwiseAnd)
            };
        }

        public override IOperable BitwiseOr(Func<IOperable> operand)
        {
            IOperable eval = operand();
            return eval.OperableType switch
            {
                ObjectType.Utf32Character => (CharacterOperable)(Value.Value | (eval as CharacterOperable).Value.Value),
                ObjectType.ArbitraryBitInteger => (BigIntOperable)(Value.Value | (eval as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => (BigIntOperable)(Value.Value | (eval as IOperable<byte>).Value),
                _ => throw MissingBinaryOperatorImplementation(eval, Models.Enums.BinaryOperator.BitwiseOr)
            };
        }

        public override IOperable BitwiseXOr(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.Utf32Character => (CharacterOperable)(Value.Value ^ (operand as CharacterOperable).Value.Value),
                ObjectType.ArbitraryBitInteger => (BigIntOperable)(Value.Value ^ (operand as IOperable<BigInteger>).Value),
                ObjectType.UnsignedByte => (BigIntOperable)(Value.Value ^ (operand as IOperable<byte>).Value),
                _ => throw MissingBinaryOperatorImplementation(operand, Models.Enums.BinaryOperator.BitwiseXOr)
            };
        }

        public override IOperable ShiftLeft(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.Utf32Character => (CharacterOperable)(Value.Value << (operand as CharacterOperable).Value.Value),
                ObjectType.ArbitraryBitInteger => Value.Value << (int)(operand as IOperable<BigInteger>).Value,
                ObjectType.UnsignedByte => Value.Value << (operand as IOperable<byte>).Value,
                _ => throw MissingBinaryOperatorImplementation(operand, Models.Enums.BinaryOperator.ShiftLeft)
            };
        }

        public override IOperable ShiftRight(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.Utf32Character => (CharacterOperable)(Value.Value >> (operand as CharacterOperable).Value.Value),
                ObjectType.ArbitraryBitInteger => Value.Value >> (int)(operand as IOperable<BigInteger>).Value,
                ObjectType.UnsignedByte => Value.Value >> (operand as IOperable<byte>).Value,
                _ => throw MissingBinaryOperatorImplementation(operand, Models.Enums.BinaryOperator.ShiftRight)
            };
        }

        public override IOperable Equal(IOperable operand)
        {
            switch (operand.OperableType)
            {
                case ObjectType.Utf32Character:
                    return BoolOperable.FromBool(Value == (operand as CharacterOperable).Value);
                case ObjectType.StringObject:
                    {
                        string s = (operand as StringObjectOperable).ToString();
                        if (s.Length == 0 || s.Length > 1)
                            return BoolOperable.False;
                        return BoolOperable.FromBool(Value == s[0]);
                    }
                case ObjectType.String:
                    {
                        string s = (operand as IOperable<string>).Value;
                        if (s.Length == 0 || s.Length > 1)
                            return BoolOperable.False;
                        return BoolOperable.FromBool(Value == s[0]);
                    }
                case ObjectType.ArbitraryBitInteger:
                    return BoolOperable.FromBool(Value.Value == (operand as IOperable<BigInteger>).Value);
                case ObjectType.UnsignedByte:
                    return BoolOperable.FromBool(Value.Value == (operand as IOperable<byte>).Value);
                case ObjectType.ArbitraryPrecisionDecimal:
                    return BoolOperable.FromBool(Value.Value == (operand as IOperable<BigDecimal>).Value);
                case ObjectType.Void:
                    return BoolOperable.False;
                default:
                    throw MissingBinaryOperatorImplementation(operand, Models.Enums.BinaryOperator.Equal);
            }
        }

        public override IOperable NotEqual(IOperable operand)
        {
            switch (operand.OperableType)
            {
                case ObjectType.Utf32Character:
                    return BoolOperable.FromBool(Value != (operand as CharacterOperable).Value);
                case ObjectType.StringObject:
                    {
                        string s = (operand as StringObjectOperable).ToString();
                        if (s.Length == 0 || s.Length > 1)
                            return BoolOperable.True;
                        return BoolOperable.FromBool(Value != s[0]);
                    }
                case ObjectType.String:
                    {
                        string s = (operand as IOperable<string>).Value;
                        if (s.Length == 0 || s.Length > 1)
                            return BoolOperable.True;
                        return BoolOperable.FromBool(Value != s[0]);
                    }
                case ObjectType.ArbitraryBitInteger:
                    return BoolOperable.FromBool(Value.Value != (operand as IOperable<BigInteger>).Value);
                case ObjectType.UnsignedByte:
                    return BoolOperable.FromBool(Value.Value != (operand as IOperable<byte>).Value);
                case ObjectType.ArbitraryPrecisionDecimal:
                    return BoolOperable.FromBool(Value.Value != (operand as IOperable<BigDecimal>).Value);
                case ObjectType.Void:
                    return BoolOperable.True;
                default:
                    throw MissingBinaryOperatorImplementation(operand, Models.Enums.BinaryOperator.NotEqual);
            }
        }

        public override IOperable<bool> StrictEqual(IOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BoolOperable.False;

            return BoolOperable.FromBool(Value == (operand as CharacterOperable).Value);
        }

        public override IOperable<bool> StrictNotEqual(IOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BoolOperable.True;

            return BoolOperable.FromBool(Value != (operand as CharacterOperable).Value);
        }

        #endregion

        #region Unary Operator Overrides

        public override IOperable UnaryMinus() => (BigIntOperable)(-Value.Value);

        #endregion

        public static implicit operator CharacterOperable(int value) => new(value);
    }
}
