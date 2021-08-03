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
    /// <summary>
    /// Represents a single value in an expression
    /// </summary>
    public class LiteralExpressionModel : IExpressionModel
    {
        public IBinaryOperable Value { get; set; }
        public IToken StartToken { get; set; }
        public IToken StopToken { get; set; }

        public LiteralExpressionModel()
        {
        }

        public LiteralExpressionModel(IBinaryOperable value) => Value = value;

        public override string ToString() => Value.Value.ToString();
    }
}
