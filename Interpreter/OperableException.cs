using Interpreter.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    public class OperableException : Exception
    {
        public OperableException()
        {
        }

        public OperableException(string message) : base(message)
        {
        }

        public OperableException(string message, Exception cause) : base(message, cause)
        {
        }
    }
}
