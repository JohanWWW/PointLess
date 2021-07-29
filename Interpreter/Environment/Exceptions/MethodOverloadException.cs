using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Environment.Exceptions
{
    public class MethodOverloadException : Exception
    {
        public MethodOverloadException() : base()
        {
        }

        public MethodOverloadException(string message) : base(message)
        {
        }

        public MethodOverloadException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
