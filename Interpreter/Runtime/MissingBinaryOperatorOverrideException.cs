using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Runtime
{
    public class MissingBinaryOperatorOverrideException : Exception
    {
        public MissingBinaryOperatorOverrideException()
        {
        }

        public MissingBinaryOperatorOverrideException(string message) : base(message)
        {
        }
    }
}
