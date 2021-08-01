using Interpreter.Environment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Runtime
{
    public class MethodDataWrapper : IBinaryOperable<MethodData> // TODO: Implement MethodDataWrapper
    {
        public MethodData Value
        {
            get => (MethodData)(this as IBinaryOperable).Value;
            set => (this as IBinaryOperable).Value = value;
        }

        public ObjectType OperableType => ObjectType.MethodData;

        object IBinaryOperable.Value { get; set; }

        public MethodDataWrapper(MethodData value) => Value = value;

        public IBinaryOperable Add(IBinaryOperable operand)
        {
            if (operand.OperableType == ObjectType.Method)
            {
                Method method = (operand as IBinaryOperable<Method>).Value;
                Value.AddOverload(method);
                return new MethodDataWrapper(Value);
            }

            throw new MissingBinaryOperatorOverrideException();
        }

        public IBinaryOperable BitwiseAnd(IBinaryOperable operand)
        {
            throw new NotImplementedException();
        }

        public IBinaryOperable BitwiseOr(IBinaryOperable operand)
        {
            throw new NotImplementedException();
        }

        public IBinaryOperable BitwiseXOr(IBinaryOperable operand)
        {
            throw new NotImplementedException();
        }

        public IBinaryOperable Divide(IBinaryOperable operand)
        {
            throw new NotImplementedException();
        }

        public IBinaryOperable Equal(IBinaryOperable operand)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public IBinaryOperable LogicalOr(IBinaryOperable operand)
        {
            throw new NotImplementedException();
        }

        public IBinaryOperable LogicalXOr(IBinaryOperable operand)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
