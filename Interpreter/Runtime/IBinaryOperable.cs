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
    public interface IBinaryOperable : IOperable
    {
        IOperable Add(IOperable operand);
        IOperable Subtract(IOperable operand);
        IOperable Multiply(IOperable operand);
        IOperable Divide(IOperable operand);
        IOperable Mod(IOperable operand);
        IOperable Equal(IOperable operand);

        /// <summary>
        /// Returns true if the two objects of the same <see cref="ObjectType"/> are equal
        /// </summary>
        IOperable<bool> StrictEqual(IOperable operand);
        IOperable NotEqual(IOperable operand);

        /// <summary>
        /// Returns false if the two objects of the same <see cref="ObjectType"/> are not equal
        /// </summary>
        IOperable<bool> StrictNotEqual(IOperable operand);
        IOperable LessThan(IOperable operand);
        IOperable LessThanOrEqual(IOperable operand);
        IOperable GreaterThan(IOperable operand);
        IOperable GreaterThanOrEqual(IOperable operand);
        IOperable LogicalAnd(Func<IOperable> operand);
        IOperable LogicalOr(Func<IOperable> operand);
        IOperable LogicalXOr(IOperable operand);
        IOperable BitwiseAnd(Func<IOperable> operand);
        IOperable BitwiseOr(Func<IOperable> operand);
        IOperable BitwiseXOr(IOperable operand);
        IOperable ShiftLeft(IOperable operand);
        IOperable ShiftRight(IOperable operand);
    }

    public interface IBinaryOperable<T> : IBinaryOperable, IOperable<T>
    {
    }
}
