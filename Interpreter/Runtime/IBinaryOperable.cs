using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Runtime
{
    /// <summary>
    /// Represents an object that can perform binary operations
    /// </summary>
    public interface IBinaryOperable
    {
        ObjectType OperableType { get; }
        object Value { get; set; }

        IBinaryOperable Add(IBinaryOperable operand);
        IBinaryOperable Subtract(IBinaryOperable operand);
        IBinaryOperable Multiply(IBinaryOperable operand);
        IBinaryOperable Divide(IBinaryOperable operand);
        IBinaryOperable Mod(IBinaryOperable operand);
        IBinaryOperable Equal(IBinaryOperable operand);
        IBinaryOperable NotEqual(IBinaryOperable operand);
        IBinaryOperable LessThan(IBinaryOperable operand);
        IBinaryOperable LessThanOrEqual(IBinaryOperable operand);
        IBinaryOperable GreaterThan(IBinaryOperable operand);
        IBinaryOperable GreaterThanOrEqual(IBinaryOperable operand);
        IBinaryOperable LogicalAnd(IBinaryOperable operand);
        IBinaryOperable LogicalOr(IBinaryOperable operand);
        IBinaryOperable LogicalXOr(IBinaryOperable operand);
        IBinaryOperable BitwiseAnd(IBinaryOperable operand);
        IBinaryOperable BitwiseOr(IBinaryOperable operand);
        IBinaryOperable BitwiseXOr(IBinaryOperable operand);
        IBinaryOperable ShiftLeft(IBinaryOperable operand);
        IBinaryOperable ShiftRight(IBinaryOperable operand);
    }

    public interface IBinaryOperable<T> : IBinaryOperable
    {
        new T Value { get; set; }
    }
}
