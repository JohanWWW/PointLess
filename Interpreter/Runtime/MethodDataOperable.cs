using Interpreter.Environment;
using Interpreter.Models.Enums;
using System;

namespace Interpreter.Runtime
{
    public class MethodDataOperable : OperableBase<MethodData>
    {
        public MethodDataOperable(MethodData value) : base(value, ObjectType.MethodData)
        {
        }

        public override IOperable Add(IOperable operand)
        {
            if (operand.OperableType == ObjectType.Method)
            {
                Method method = (operand as IOperable<Method>).Value;
                Value.AddOverload(method);
                return new MethodDataOperable(Value);
            }

            throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Add);
        }

        public override IOperable Equal(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.MethodData => BoolOperable.FromBool(Value == operand.Value),
                ObjectType.Void => BoolOperable.False,
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.Equal)
            };
        }

        public override IOperable NotEqual(IOperable operand)
        {
            return operand.OperableType switch
            {
                ObjectType.MethodData => BoolOperable.FromBool(Value != operand.Value),
                ObjectType.Void => BoolOperable.True,
                _ => throw MissingBinaryOperatorImplementation(operand, BinaryOperator.NotEqual)
            };
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

        public static implicit operator MethodDataOperable(MethodData value) => new MethodDataOperable(value);

        public override string ToString()
        {
            if (!Value.TryGetSingle(out Method method))
                return Value.ToString();

            return method.MethodType switch
            {
                MethodType.Action   => "()=>void",
                MethodType.Consumer => $"([{method.ParameterCount}])=>void",
                MethodType.Function => $"([{method.ParameterCount}])=>*",
                MethodType.Provider => $"()=>*",
                _                   => method.ToString()
            };
        }

        #region IConvertible implementations
        public override TypeCode GetTypeCode() => TypeCode.Object;
        #endregion
    }
}
