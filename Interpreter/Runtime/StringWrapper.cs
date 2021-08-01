using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Runtime
{
    // TODO: Use string wrapper instead of string
    public class StringWrapper : IBinaryOperable<string>
    {
        public ObjectType OperableType => ObjectType.String;
        
        object IBinaryOperable.Value { get; set; }

        public string Value
        {
            get => (string)(this as IBinaryOperable).Value;
            set => (this as IBinaryOperable).Value = value;
        }

        public StringWrapper(string value) => Value = value;

        public IBinaryOperable Add(IBinaryOperable operand)
        {
            return new StringWrapper(Value + operand.ToString());
        }

        public IBinaryOperable Subtract(IBinaryOperable operand) => throw new MissingBinaryOperatorOverrideException();

        public IBinaryOperable Multiply(IBinaryOperable operand) => throw new MissingBinaryOperatorOverrideException();

        public IBinaryOperable Divide(IBinaryOperable operand) => throw new MissingBinaryOperatorOverrideException();

        public IBinaryOperable Mod(IBinaryOperable operand) => throw new MissingBinaryOperatorOverrideException();

        public IBinaryOperable Equal(IBinaryOperable operand) => throw new MissingBinaryOperatorOverrideException();

        public IBinaryOperable NotEqual(IBinaryOperable operand) => throw new MissingBinaryOperatorOverrideException();

        public IBinaryOperable LessThan(IBinaryOperable operand) => throw new MissingBinaryOperatorOverrideException();

        public IBinaryOperable LessThanOrEqual(IBinaryOperable operand) => throw new MissingBinaryOperatorOverrideException();

        public IBinaryOperable GreaterThan(IBinaryOperable operand) => throw new MissingBinaryOperatorOverrideException();

        public IBinaryOperable GreaterThanOrEqual(IBinaryOperable operand) => throw new MissingBinaryOperatorOverrideException();

        public IBinaryOperable LogicalAnd(IBinaryOperable operand) => throw new MissingBinaryOperatorOverrideException();

        public IBinaryOperable LogicalOr(IBinaryOperable operand) => throw new MissingBinaryOperatorOverrideException();

        public IBinaryOperable LogicalXOr(IBinaryOperable operand) => throw new MissingBinaryOperatorOverrideException();

        public IBinaryOperable BitwiseAnd(IBinaryOperable operand) => throw new MissingBinaryOperatorOverrideException();

        public IBinaryOperable BitwiseOr(IBinaryOperable operand) => throw new MissingBinaryOperatorOverrideException();

        public IBinaryOperable BitwiseXOr(IBinaryOperable operand) => throw new MissingBinaryOperatorOverrideException();

        public IBinaryOperable ShiftLeft(IBinaryOperable operand) => throw new MissingBinaryOperatorOverrideException();

        public IBinaryOperable ShiftRight(IBinaryOperable operand) => throw new MissingBinaryOperatorOverrideException();

        public override string ToString() => Value;
    }
}
