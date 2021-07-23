using Antlr4.Runtime;
using Interpreter.Models.Enums;
using Interpreter.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Models
{
    /// <summary>
    /// Represents a binary expression
    /// </summary>
    public class BinaryExpressionModel : IExpressionModel
    {
        public BinaryOperator Operator { get; set; }
        public IExpressionModel LeftExpression { get; set; }
        public IExpressionModel RightExpression { get; set; }
        public IToken StartToken { get; set; }
        public IToken StopToken { get; set; }
    }
}
