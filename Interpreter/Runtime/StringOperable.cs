using Interpreter.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Runtime
{
    public class StringOperable : OperableBase<string>
    {
        public StringOperable(string value) : base(value, ObjectType.String)
        {
        }

        public override IOperable Add(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.String => new StringOperable(Value + (operand as IBinaryOperable<string>).Value),
                _ => new StringOperable(Value + operand.ToString())
            };
        }

        public override IOperable Equal(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.String => BoolOperable.FromBool(Value == (operand as IBinaryOperable<string>).Value),
                ObjectType.NullReference => BoolOperable.FromBool(Value == null),
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Equal)
            };
        }

        public override IOperable NotEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.String => BoolOperable.FromBool(Value != (operand as IBinaryOperable<string>).Value),
                ObjectType.NullReference => BoolOperable.FromBool(Value != null),
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.NotEqual)
            };
        }

        public override IOperable<bool> StrictEqual(IOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BoolOperable.False;

            return BoolOperable.FromBool(Value == (string)operand.Value);
        }

        public override IOperable<bool> StrictNotEqual(IOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BoolOperable.True;

            return BoolOperable.FromBool(Value != (string)operand.Value);
        }

        public static implicit operator StringOperable(string value) => new StringOperable(value);
    }
}
