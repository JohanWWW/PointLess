using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Runtime
{
    public interface IOperable
    {
        ObjectType OperableType { get; }
        object Value { get; }
    }

    public interface IOperable<T> : IOperable
    {
        new T Value => (T)(this as IOperable).Value;
    }
}
