using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Runtime
{
    /// <summary>
    /// Represents a value from a literal or a variable
    /// </summary>
    public interface IOperable : IValueConvertible
    {
        ObjectType OperableType { get; }
        object Value { get; }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <typeparam name="T">The generic type of the represented value</typeparam>
    public interface IOperable<T> : IOperable
    {
        new T Value => (T)(this as IOperable).Value;
    }
}
