using System;
using System.Collections.Generic;

namespace Interpreter.Runtime
{
    [Obsolete("Use " + nameof(DictionaryObjectOperable) + " instead")]
    public class DictionaryOperable : OperableBase<IDictionary<IOperable, IOperable>>
    {
        public DictionaryOperable(IDictionary<IOperable, IOperable> value) : base(value, ObjectType.Dictionary)
        {
        }

        public override TypeCode GetTypeCode()
        {
            throw new NotImplementedException();
        }

        public override IOperable<bool> StrictEqual(IOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BoolOperable.False;

            return BoolOperable.FromBool(Value == operand.Value);
        }

        public override IOperable<bool> StrictNotEqual(IOperable operand)
        {
            if (OperableType != operand.OperableType)
                return BoolOperable.True;

            return BoolOperable.FromBool(Value != operand.Value);
        }
    }
}
