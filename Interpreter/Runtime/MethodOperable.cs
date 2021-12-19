using Interpreter.Environment;
using Interpreter.Models.Enums;
using System;

namespace Interpreter.Runtime
{
    public class MethodOperable : OperableBase<Method>
    {
        public MethodOperable(Method value) : base(value, ObjectType.Method)
        {
        }

        public override IOperable Add(IOperable operand)
        {
            if (operand.OperableType == ObjectType.MethodData)
            {
                MethodData methodData = (operand as IOperable<MethodData>).Value;
                methodData.AddOverload(Value);
                return new MethodDataOperable(methodData);
            }
            else if (operand.OperableType == ObjectType.Method)
            {
                MethodData methodData = new(Value);
                methodData.AddOverload((operand as IOperable<Method>).Value);
                //return new MethodDataOperable(new MethodData((operand as IOperable<Method>).Value));
                return (MethodDataOperable)methodData;
            }

            throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Add);
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

        #region IConvertible implementations
        public override TypeCode GetTypeCode() => TypeCode.Object;
        #endregion
    }
}
