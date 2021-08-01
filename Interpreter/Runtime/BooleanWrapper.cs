using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Runtime
{
    public class BooleanWrapper : IBinaryOperable<bool> // TODO: Implement BooleanWrapper
    {
        public bool Value
        {
            get => (bool)(this as IBinaryOperable).Value;
            set => (this as IBinaryOperable).Value = value;
        }

        public ObjectType OperableType => ObjectType.Boolean;

        object IBinaryOperable.Value { get; set; }

        public BooleanWrapper(bool value) => Value = value;

        public IBinaryOperable Add(IBinaryOperable operand)
        {
            throw new NotImplementedException();
        }

        public IBinaryOperable BitwiseAnd(IBinaryOperable operand) => LogicalAnd(operand);

        public IBinaryOperable BitwiseOr(IBinaryOperable operand) => LogicalOr(operand);

        public IBinaryOperable BitwiseXOr(IBinaryOperable operand) => LogicalXOr(operand);

        public IBinaryOperable Divide(IBinaryOperable operand)
        {
            throw new NotImplementedException();
        }

        public IBinaryOperable Equal(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.Boolean => new BooleanWrapper(Value == (operand as IBinaryOperable<bool>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public IBinaryOperable GreaterThan(IBinaryOperable operand)
        {
            throw new NotImplementedException();
        }

        public IBinaryOperable GreaterThanOrEqual(IBinaryOperable operand)
        {
            throw new NotImplementedException();
        }

        public IBinaryOperable LessThan(IBinaryOperable operand)
        {
            throw new NotImplementedException();
        }

        public IBinaryOperable LessThanOrEqual(IBinaryOperable operand)
        {
            throw new NotImplementedException();
        }

        public IBinaryOperable LogicalAnd(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.Boolean => new BooleanWrapper(Value && (operand as IBinaryOperable<bool>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public IBinaryOperable LogicalOr(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.Boolean => new BooleanWrapper(Value || (operand as IBinaryOperable<bool>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public IBinaryOperable LogicalXOr(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.Boolean => new BooleanWrapper(Value ^ (operand as IBinaryOperable<bool>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public IBinaryOperable Mod(IBinaryOperable operand)
        {
            throw new NotImplementedException();
        }

        public IBinaryOperable Multiply(IBinaryOperable operand)
        {
            throw new NotImplementedException();
        }

        public IBinaryOperable NotEqual(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.Boolean => new BooleanWrapper(Value != (operand as IBinaryOperable<bool>).Value),
                _ => throw new MissingBinaryOperatorOverrideException()
            };
        }

        public IBinaryOperable ShiftLeft(IBinaryOperable operand)
        {
            throw new NotImplementedException();
        }

        public IBinaryOperable ShiftRight(IBinaryOperable operand)
        {
            throw new NotImplementedException();
        }

        public IBinaryOperable Subtract(IBinaryOperable operand)
        {
            throw new NotImplementedException();
        }

        public override string ToString() => Value.ToString();
    }
}
