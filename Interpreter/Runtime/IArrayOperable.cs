using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Runtime
{
    public interface IArrayOperable<T> : IOperable<T[]> where T : IOperable
    {
        T[] GetArray();
    }

    public interface IArrayOperable : IArrayOperable<IOperable>
    {
        new IOperable[] GetArray();
    }
}
