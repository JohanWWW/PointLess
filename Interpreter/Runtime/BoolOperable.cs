using Interpreter.Models.Enums;
using Singulink.Numerics;
using System;
using System.Numerics;

namespace Interpreter.Runtime
{
    public class BoolOperable : OperableBase<bool>
    {
        private const string TRUE_SYMBOL = "true";
        private const string FALSE_SYMBOL = "false";

        public static readonly BoolOperable True = new(true);
        public static readonly BoolOperable False = new(false);

        private BoolOperable(bool value) : base(value, ObjectType.Boolean)
        {
        }

        public static BoolOperable FromBool(bool value) => value ? True : False;

        #region Binary Operators

        public override IOperable BitwiseAnd(Func<IOperable> operand) => LogicalAnd(operand);

        public override IOperable BitwiseOr(Func<IOperable> operand) => LogicalOr(operand);

        public override IOperable BitwiseXOr(IOperable operand) => LogicalXOr(operand);

        public override IOperable Equal(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.Boolean => FromBool(Value == (operand as IOperable<bool>).Value),
                ObjectType.Void => False,
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Equal)
            };
        }

        public override IOperable LogicalAnd(Func<IOperable> operand)
        {
            if (!Value)
                return False;

            IOperable eval = operand();

            return eval.OperableType switch
            {
                ObjectType.Boolean => FromBool(Value && (eval as IOperable<bool>).Value),
                _ => throw MissingBinaryOperatorImplementation(eval, BinaryOperator.LogicalAnd)
            };
        }

        public override IOperable LogicalOr(Func<IOperable> operand)
        {
            if (Value)
                return True;

            IOperable eval = operand();

            return eval.OperableType switch
            {
                ObjectType.Boolean => FromBool(Value || (eval as IOperable<bool>).Value),
                _ => throw MissingBinaryOperatorImplementation(eval, BinaryOperator.LogicalOr)
            };
        }

        public override IOperable LogicalXOr(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.Boolean => FromBool(Value ^ (operand as IOperable<bool>).Value),
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.LogicalXOr)
            };
        }

        public override IOperable NotEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.Boolean => FromBool(Value != (operand as IOperable<bool>).Value),
                ObjectType.Void => True,
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.NotEqual)
            };
        }

        public override IOperable<bool> StrictEqual(IOperable operand)
        {
            if (OperableType != operand.OperableType)
                return False;

            return FromBool(Value == (bool)operand.Value);
        }

        public override IOperable<bool> StrictNotEqual(IOperable operand)
        {
            if (OperableType != operand.OperableType)
                return True;

            return FromBool(Value != (bool)operand.Value);
        }

        #endregion

        #region Unary Operators

        public override IOperable UnaryNot() => FromBool(!Value);

        #endregion

        public static implicit operator BoolOperable(bool value) => FromBool(value);

        public override string ToString() => Value ? TRUE_SYMBOL : FALSE_SYMBOL;

        #region Convertible Implementations
        public override TypeCode GetTypeCode() => TypeCode.Boolean;
        public override bool ToBoolean(IFormatProvider provider) => Value;
        public override BigDecimal ToBigDecimal(IFormatProvider provider) => Value ? 1 : 0;
        public override BigInteger ToBigInteger(IFormatProvider provider) => Value ? 1 : 0;
        public override byte ToByte(IFormatProvider provider) => Value ? (byte)1 : (byte)0;
        public override char ToChar(IFormatProvider provider) => Value ? (char)1 : (char)0;
        public override decimal ToDecimal(IFormatProvider provider) => Value ? 1m : 0m;
        public override double ToDouble(IFormatProvider provider) => Value ? 1d : 0d;
        public override short ToInt16(IFormatProvider provider) => Value ? (short)1 : (short)0;
        public override int ToInt32(IFormatProvider provider) => Value ? 1 : 0;
        public override long ToInt64(IFormatProvider provider) => Value ? 1 : 0;
        public override sbyte ToSByte(IFormatProvider provider) => Value ? (sbyte)1 : (sbyte)0;
        public override float ToSingle(IFormatProvider provider) => Value ? 1f : 0f;
        public override ushort ToUInt16(IFormatProvider provider) => Value ? (ushort)1 : (ushort)0;
        public override uint ToUInt32(IFormatProvider provider) => Value ? (uint)1 : 0;
        public override ulong ToUInt64(IFormatProvider provider) => Value ? (ulong)1 : 0;
        public override string ToString(IFormatProvider provider) => ToString();
        #endregion
    }
}
