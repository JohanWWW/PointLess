using Antlr4.Runtime;
using Interpreter.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    public class InterpreterRuntimeException : Exception
    {
        private const string _DEFAULT_MESSAGE = "A runtime exception was thrown";

        private readonly string _message;

        internal IToken StartToken { get; private set; }
        internal IToken StopToken { get; private set; }

        public int LineNumber => StartToken.Line;
        public int StartColumn => StartToken.Column;
        public int StopColumn => StopToken.Column;
        public string AffectedFilePath { get; private set; }
        public Exception CausedBy { get; private set; }

        public bool IsSingleCharacterToken => StartColumn == StopColumn;
        
        public override string Message => _message;

        internal InterpreterRuntimeException(IModel runtimeModel, string filePath) : this(runtimeModel, filePath, _DEFAULT_MESSAGE)
        {
        }

        internal InterpreterRuntimeException(IModel runtimeModel, string filePath, string message) : this(runtimeModel, filePath, message, null)
        {
        }

        internal InterpreterRuntimeException(IModel runtimeModel, string filePath, string message, Exception innerCause) : this(runtimeModel.StartToken, runtimeModel.StopToken, filePath, message, innerCause)
        {
        }


        internal InterpreterRuntimeException(IToken startToken, IToken stopToken, string filePath) : this(startToken, stopToken,filePath, _DEFAULT_MESSAGE)
        {
        }

        internal InterpreterRuntimeException(IToken startToken, IToken stopToken, string filePath, string message) : this(startToken, stopToken, filePath, message, null)
        {
        }

        internal InterpreterRuntimeException(IToken startToken, IToken stopToken, string filePath, string message, Exception innerCause)
        {
            StartToken = startToken;
            StopToken = stopToken;
            AffectedFilePath = filePath;
            CausedBy = innerCause;
            _message = message;
        }

        public override string ToString() => Message + $"\n\tat {AffectedFilePath}:{LineNumber}:{StartColumn}";
    }
}
