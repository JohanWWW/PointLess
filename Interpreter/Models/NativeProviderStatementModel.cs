using Antlr4.Runtime;
using Interpreter.Models.Interfaces;
using Interpreter.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Models
{
    public class NativeProviderStatementModel : IFunctionModel
    {
        public Func<IBinaryOperable> NativeImplementation { get; set; }
        public IToken StartToken { get; set; }
        public IToken StopToken { get; set; }
    }
}
