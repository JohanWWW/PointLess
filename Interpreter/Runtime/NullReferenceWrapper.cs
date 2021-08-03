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

        public override IBinaryOperable Equal(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.NullReference => BooleanWrapper.True,
                _ => BooleanWrapper.False
            };
        }

        public override IBinaryOperable NotEqual(IBinaryOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.NullReference => BooleanWrapper.False,
                _ => BooleanWrapper.True
            };
        }

        public override IBinaryOperable<bool> StrictEqual(IBinaryOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BooleanWrapper.False;

            return (IBinaryOperable<bool>)Equal(operand);
        }

        public override IBinaryOperable<bool> StrictNotEqual(IBinaryOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BooleanWrapper.True;

            return (IBinaryOperable<bool>)NotEqual(operand);
        }

        public override string ToString() => NULL_SYMBOL;
    }
}
