using Interpreter.Models.Interfaces;
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
        public dynamic Value { get; set; }

        public LiteralExpressionModel()
        {
        }

        public LiteralExpressionModel(dynamic value) => Value = value;

        public override string ToString() => Value;
    }
}
