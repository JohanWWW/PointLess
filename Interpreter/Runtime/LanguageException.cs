using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Runtime
{
    public class LanguageException : Exception
    {
        public object Argument { get; private set; }
        public LanguageException InnerLanguageException { get; private set; }

        public LanguageException()
        {
        }

        public LanguageException(object obj) : base(obj.ToString())
        {
            Argument = obj;
        }

        public LanguageException(object obj, LanguageException innerLanguageException) : base(obj.ToString(), innerLanguageException.InnerException)
        {
            Argument = obj;
            InnerLanguageException = innerLanguageException;
        }
    }
}
