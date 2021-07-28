using Antlr4.Runtime;
using Interpreter.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Models
{
    public class LambdaFunctionStatementModel : IFunctionModel
    {
        public string[] Parameters { get; set; }
        public IExpressionModel Return { get; set; }
        public AssignStatementModel AssignStatement { get; set; }
        public IToken StartToken { get; set; }
        public IToken StopToken { get; set; }

        public bool IsModeReturn => Return != null;
        public IModel Mode => IsModeReturn ? Return : AssignStatement;
    }
}
