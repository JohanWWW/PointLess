using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    public class NativeImplementationException : Exception
    {
        public NativeImplementationException()
        {
        }

        public NativeImplementationException(string message) : base(message)
        {
        }

        public NativeImplementationException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
