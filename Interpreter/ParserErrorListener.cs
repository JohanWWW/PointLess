using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    public class ParserErrorListener : BaseErrorListener
    {
        public override void SyntaxError([NotNull] IRecognizer recognizer, 
                                         [Nullable] IToken offendingSymbol, 
                                         int line, 
                                         int charPositionInLine, 
                                         [NotNull] string msg, 
                                         [Nullable] RecognitionException e)
        {
            throw new LanguageSyntaxException(line, charPositionInLine, charPositionInLine + offendingSymbol.Text.Length);
        }
    }
}
