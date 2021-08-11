using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Runtime
{
    public interface IUnaryOperable : IOperable
    {
        IOperable UnaryMinus();
        IOperable UnaryNot();
    }

    public interface IUnaryOperable<T> : IUnaryOperable, IOperable<T>
    {
    }
}
