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
    public class NativeFunctionStatementModel : IFunctionModel
    {
        public string[] Parameters { get; set; }
        public Func<IList<IBinaryOperable>, IBinaryOperable> NativeImplementation { get; set; }
        public IToken StartToken { get; set; }
        public IToken StopToken { get; set; }

        public NativeFunctionStatementModel()
        {
        }

        public NativeFunctionStatementModel(params string[] parameters) => Parameters = parameters;
    }
}
