using Interpreter.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Runtime
{
    public class NullOperable : OperableBase
    {
        private const string NULL_SYMBOL = "null";

        public static readonly NullOperable Null = new();

        private NullOperable() : base(null, ObjectType.NullReference)
        {
        }

        public override IOperable Equal(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.NullReference => BoolOperable.True,
                _ => BoolOperable.False
            };
        }

        public override IOperable NotEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.NullReference => BoolOperable.False,
                _ => BoolOperable.True
            };
        }

        public override IOperable<bool> StrictEqual(IOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BoolOperable.False;

            return (IBinaryOperable<bool>)Equal(operand);
        }

        public override IOperable<bool> StrictNotEqual(IOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BoolOperable.True;

            return (IBinaryOperable<bool>)NotEqual(operand);
        }

        public override string ToString() => NULL_SYMBOL;
    }
}
