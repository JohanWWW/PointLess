using Interpreter.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Runtime
{
    public class StringWrapper : WrapperBase<string>
    {
        public StringWrapper(string value) : base(value, ObjectType.String)
        {
        }

        public override IBinaryOperable Add(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.String => new StringWrapper(Value + (operand as IBinaryOperable<string>).Value),
                _ => new StringWrapper(Value + operand.ToString())
            };
        }

        public override IBinaryOperable Equal(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.String => BooleanWrapper.FromBool(Value == (operand as IBinaryOperable<string>).Value),
                ObjectType.NullReference => BooleanWrapper.FromBool(Value == null),
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Equal)
            };
        }

        public override IBinaryOperable NotEqual(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.String => BooleanWrapper.FromBool(Value != (operand as IBinaryOperable<string>).Value),
                ObjectType.NullReference => BooleanWrapper.FromBool(Value != null),
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.NotEqual)
            };
        }

        public override IBinaryOperable<bool> StrictEqual(IBinaryOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BooleanWrapper.False;

            return BooleanWrapper.FromBool(Value == (string)operand.Value);
        }

        public override IBinaryOperable<bool> StrictNotEqual(IBinaryOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BooleanWrapper.True;

            return BooleanWrapper.FromBool(Value != (string)operand.Value);
        }
    }
}
