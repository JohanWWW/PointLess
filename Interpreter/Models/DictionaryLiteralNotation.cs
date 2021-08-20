using Interpreter.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Models
{
    public class DictionaryLiteralNotation : ModelBase, IExpressionModel
    {
        public (IExpressionModel key, IExpressionModel value)[] Arguments { get; set; }

        public DictionaryLiteralNotation() : base(Enums.ModelTypeCode.DictionaryLiteralNotation)
        {
        }
    }
}
