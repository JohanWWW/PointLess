using Interpreter.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Models
{
    public class ArrayLiteralNotationModel : ModelBase, IExpressionModel
    {
        public IExpressionModel[] Arguments { get; set; }

        public ArrayLiteralNotationModel() : base(Enums.ModelTypeCode.ArrayLiteralNotation)
        {
        }
    }
}
