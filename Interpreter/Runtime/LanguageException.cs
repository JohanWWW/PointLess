using Antlr4.Runtime;
using Interpreter.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Runtime
{
    // TODO: Almost indifferent to InterpreterRuntimeException. Remove.
    public class LanguageException : Exception
    {
        private string _message;

        internal IToken StartToken { get; private set; }
        internal IToken StopToken { get; private set; }

        /// <summary>
        /// The argument that was passed in the throw statement
        /// </summary>
        public IOperable Argument { get; private set; }
        public string FilePath { get; private set; }

        public int LineNumber => StartToken.Line;
        public int StartColumn => StartToken.Column;
        public int StopColumn => StopToken.Column;

        public override string Message => _message ?? Argument.ToString();

        public LanguageException(IOperable arg, IModel runtimeModel, string filePath, string message) : this(arg, runtimeModel, filePath)
        {
            _message = message;
        }

        public LanguageException(IOperable arg, IModel runtimeModel, string filePath)
        {
            StartToken = runtimeModel.StartToken;
            StopToken = runtimeModel.StopToken;
            Argument = arg;
            FilePath = filePath;
        }
    }
}
