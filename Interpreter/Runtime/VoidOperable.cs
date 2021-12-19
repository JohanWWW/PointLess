using System;

namespace Interpreter.Runtime
{
    public class VoidOperable : OperableBase
    {
        private const string VOID_SYMBOL = "void";

        public static readonly VoidOperable Void = new();

        private VoidOperable() : base(null, ObjectType.Void)
        {
        }

        public override IOperable Equal(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.Void => BoolOperable.True,
                _ => BoolOperable.False
            };
        }

        public override IOperable NotEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.Void => BoolOperable.False,
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

        public override string ToString() => VOID_SYMBOL;

        #region IConvertible implementations
        public override TypeCode GetTypeCode() => TypeCode.Empty;
        public override string ToString(IFormatProvider provider) => ToString();
        #endregion
    }
}
