using Antlr4.Runtime;
using Interpreter.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Models
{
    public class NativeConsumerStatementModel : IFunctionModel
    {
        public ParameterListModel Parameters { get; set; }
        public Action<IList<dynamic>> NativeImplementation { get; set; }
        public IToken StartToken { get; set; }
        public IToken StopToken { get; set; }
    }
}
