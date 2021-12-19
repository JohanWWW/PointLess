using Interpreter.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Runtime
{
    /// <summary>
    /// Represents the native array that is encapsulated by the framework's own Array class
    /// </summary>
    [Obsolete("Use " + nameof(ArrayObjectOperable) + " instead")]
    public class ArrayOperable : OperableBase<IOperable[]>
    {
        public ArrayOperable(IOperable[] value) : base(value, ObjectType.Array)
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
