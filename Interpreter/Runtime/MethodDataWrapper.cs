using Interpreter.Environment;
using Interpreter.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Runtime
{
    public class MethodDataWrapper : WrapperBase<MethodData>
    {
        public MethodDataWrapper(MethodData value) : base(value, ObjectType.MethodData)
        {
        }

        public override IBinaryOperable Add(IBinaryOperable operand)
        {
            if (operand.OperableType == ObjectType.Method)
            {
                Method method = (operand as IBinaryOperable<Method>).Value;
                Value.AddOverload(method);
                return new MethodDataWrapper(Value);
            }

            throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Add);
        }

        public override IBinaryOperable Equal(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.MethodData => BooleanWrapper.FromBool(Value == operand.Value),
                ObjectType.NullReference => BooleanWrapper.False,
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Equal)
            };
        }

        public override IBinaryOperable NotEqual(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.MethodData => BooleanWrapper.FromBool(Value != operand.Value),
                ObjectType.NullReference => BooleanWrapper.True,
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.NotEqual)
            };
        }

        public override IBinaryOperable<bool> StrictEqual(IBinaryOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BooleanWrapper.False;

            return BooleanWrapper.FromBool(Value == operand.Value);
        }

        public override IBinaryOperable<bool> StrictNotEqual(IBinaryOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BooleanWrapper.True;

            return BooleanWrapper.FromBool(Value != operand.Value);
        }

        public override string ToString() => Value.ToString();
    }
}
