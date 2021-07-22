using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    public class LanguageSyntaxException : Exception
    {
        public int Line { get; private set; }
        public int StartColumn { get; private set; }
        public int EndColumn { get; private set; }

        public LanguageSyntaxException(int line, int startColumn, int endColumn) : base($"Syntax error at {line}:{startColumn}")
        {
            Line = line;
            StartColumn = startColumn;
            EndColumn = endColumn;
        }

        public override string ToString() => Message ?? base.ToString();
    }
}
