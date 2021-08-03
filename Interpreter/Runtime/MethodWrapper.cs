using Interpreter.Environment;
using Interpreter.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Runtime
{
    public class MethodWrapper : WrapperBase<Method>
    {
        public MethodWrapper(Method value) : base(value, ObjectType.Method)
        {
        }

        public override IBinaryOperable Add(IBinaryOperable operand)
        {
            if (operand.OperableType == ObjectType.MethodData)
            {
                MethodData methodData = (operand as IBinaryOperable<MethodData>).Value;
                methodData.AddOverload(Value);
                return new MethodDataWrapper(methodData);
            }
            else if (operand.OperableType == ObjectType.Method)
            {
                return new MethodDataWrapper(new MethodData((operand as IBinaryOperable<Method>).Value));
            }

            throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Add);
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
    }
}
