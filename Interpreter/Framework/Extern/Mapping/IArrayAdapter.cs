using Interpreter.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Framework.Extern.Mapping
{
    public interface IArrayAdapter : IEnumerable
    {
        IArrayOperable OperableReference { get; }
        int Length { get; }
        object this[int index] { get; set; }

        public T ElementAt<T>(int index);
    }

    public interface IArrayAdapter<T> : IArrayAdapter, IEnumerable<T>
    {
        new T this[int index] { get; set; }
    }
}
