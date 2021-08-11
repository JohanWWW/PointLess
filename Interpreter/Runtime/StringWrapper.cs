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

        public override IOperable Add(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.String => new StringWrapper(Value + (operand as IBinaryOperable<string>).Value),
                _ => new StringWrapper(Value + operand.ToString())
            };
        }

        public override IOperable Equal(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.String => BooleanWrapper.FromBool(Value == (operand as IBinaryOperable<string>).Value),
                ObjectType.NullReference => BooleanWrapper.FromBool(Value == null),
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Equal)
            };
        }

        public override IOperable NotEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.String => BooleanWrapper.FromBool(Value != (operand as IBinaryOperable<string>).Value),
                ObjectType.NullReference => BooleanWrapper.FromBool(Value != null),
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.NotEqual)
            };
        }

        public override IOperable<bool> StrictEqual(IOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BooleanWrapper.False;

            return BooleanWrapper.FromBool(Value == (string)operand.Value);
        }

        public override IOperable<bool> StrictNotEqual(IOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BooleanWrapper.True;

            return BooleanWrapper.FromBool(Value != (string)operand.Value);
        }
    }
}
