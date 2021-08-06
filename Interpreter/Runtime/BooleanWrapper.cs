using Interpreter.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Runtime
{
    public class BooleanWrapper : WrapperBase<bool>
    {
        private const string TRUE_SYMBOL = "true";
        private const string FALSE_SYMBOL = "false";

        public static readonly BooleanWrapper True = new(true);
        public static readonly BooleanWrapper False = new(false);

        private BooleanWrapper(bool value) : base(value, ObjectType.Boolean)
        {
        }

        public static BooleanWrapper FromBool(bool value) => value ? True : False;

        public override IBinaryOperable BitwiseAnd(Func<IBinaryOperable> operand) => LogicalAnd(operand);

        public override IBinaryOperable BitwiseOr(Func<IBinaryOperable> operand) => LogicalOr(operand);

        public override IBinaryOperable BitwiseXOr(IBinaryOperable operand) => LogicalXOr(operand);

        public override IBinaryOperable Equal(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.Boolean => FromBool(Value == (operand as IBinaryOperable<bool>).Value),
                ObjectType.NullReference => False,
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Equal)
            };
        }

        public override IBinaryOperable LogicalAnd(Func<IBinaryOperable> operand)
        {
            if (!Value)
                return False;

            IBinaryOperable eval = operand();

            return eval.OperableType switch
            {
                ObjectType.Boolean => FromBool(Value && (eval as IBinaryOperable<bool>).Value),
                _ => throw MissingBinaryOperatorImplementation(eval, BinaryOperator.LogicalAnd)
            };
        }

        public override IBinaryOperable LogicalOr(Func<IBinaryOperable> operand)
        {
            if (Value)
                return True;

            IBinaryOperable eval = operand();

            return eval.OperableType switch
            {
                ObjectType.Boolean => FromBool(Value || (eval as IBinaryOperable<bool>).Value),
                _ => throw MissingBinaryOperatorImplementation(eval, BinaryOperator.LogicalOr)
            };
        }

        public override IBinaryOperable LogicalXOr(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.Boolean => FromBool(Value ^ (operand as IBinaryOperable<bool>).Value),
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.LogicalXOr)
            };
        }

        public override IBinaryOperable NotEqual(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.Boolean => FromBool(Value != (operand as IBinaryOperable<bool>).Value),
                ObjectType.NullReference => True,
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.NotEqual)
            };
        }

        public override IBinaryOperable<bool> StrictEqual(IBinaryOperable operand)
        {
            if (OperableType != operand.OperableType)
                return False;

            return FromBool(Value == (bool)operand.Value);
        }

        public override IBinaryOperable<bool> StrictNotEqual(IBinaryOperable operand)
        {
            if (OperableType != operand.OperableType)
                return True;

            return FromBool(Value != (bool)operand.Value);
        }

        public override string ToString() => Value ? TRUE_SYMBOL : FALSE_SYMBOL;
    }
}
