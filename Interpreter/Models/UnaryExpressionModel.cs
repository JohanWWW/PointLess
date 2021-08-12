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
    /// Represents a unary expression
    /// </summary>
    public class UnaryExpressionModel : ModelBase, IExpressionModel
    {
        public UnaryOperator Operator { get; set; }
        public IExpressionModel Expression { get; set; }

        public UnaryExpressionModel() : base(ModelTypeCode.UnaryExpression)
        {
        }
    }
}
