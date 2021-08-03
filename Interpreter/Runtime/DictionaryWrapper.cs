using Interpreter.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Runtime
{
    public class DictionaryWrapper : WrapperBase<IDictionary<IBinaryOperable, IBinaryOperable>>
    {
        public DictionaryWrapper(IDictionary<IBinaryOperable, IBinaryOperable> value) : base(value, ObjectType.Dictionary)
        {
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
