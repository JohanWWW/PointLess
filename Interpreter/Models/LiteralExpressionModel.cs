using Antlr4.Runtime;
using Interpreter.Models.Enums;
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
    public class LiteralExpressionModel : ModelBase, IExpressionModel
    {
        public IBinaryOperable Value { get; set; }

        public LiteralExpressionModel() : this(null, null, null)
        {
        }

        public LiteralExpressionModel(IBinaryOperable value) : this(value, null, null)
        {
        }

        public LiteralExpressionModel(IBinaryOperable value, IToken start, IToken stop) : base(ModelTypeCode.LiteralExpression, start, stop)
        {
            Value = value;
        }

        public override string ToString() => Value.Value.ToString();
    }
}
