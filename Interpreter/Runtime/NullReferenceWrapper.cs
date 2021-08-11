using Interpreter.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Runtime
{
    public class NullReferenceWrapper : WrapperBase
    {
        private const string NULL_SYMBOL = "null";

        public static readonly NullReferenceWrapper Null = new();

        private NullReferenceWrapper() : base(null, ObjectType.NullReference)
        {
        }

        public override IOperable Equal(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.NullReference => BooleanWrapper.True,
                _ => BooleanWrapper.False
            };
        }

        public override IOperable NotEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.NullReference => BooleanWrapper.False,
                _ => BooleanWrapper.True
            };
        }

        public override IOperable<bool> StrictEqual(IOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BooleanWrapper.False;

            return (IBinaryOperable<bool>)Equal(operand);
        }

        public override IOperable<bool> StrictNotEqual(IOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BooleanWrapper.True;

            return (IBinaryOperable<bool>)NotEqual(operand);
        }

        public override string ToString() => NULL_SYMBOL;
    }
}
