using Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroPointCLI
{
    public class SyntaxException : Exception
    {
        public string AffectedFilePath { get; private set; }
        public LanguageSyntaxException LanguageSyntaxException { get; private set; }

        public override string Message => $"Syntax exception at {AffectedFilePath}:{LanguageSyntaxException.Line}:{LanguageSyntaxException.StartColumn}";

        public SyntaxException(string affectedFilePath, LanguageSyntaxException languageSyntaxException)
        {
            AffectedFilePath = affectedFilePath;
            LanguageSyntaxException = languageSyntaxException;
        }

        public override string ToString() => Message;
    }
}
