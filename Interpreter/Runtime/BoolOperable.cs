using Interpreter.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public override string ToString() => Value ? TRUE_SYMBOL : FALSE_SYMBOL;
    }
}
