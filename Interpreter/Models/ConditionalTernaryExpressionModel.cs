using Antlr4.Runtime;
using Interpreter.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Models
{
    public class ConditionalTernaryExpressionModel : ITernaryExpressionModel
    {
        public IExpressionModel ConditionExpression { get; set; }
        public IExpressionModel TrueExpression { get; set; }
        public IExpressionModel FalseExpression { get; set; }

        public IToken StartToken { get; set; }
        public IToken StopToken { get; set; }
    }
}
