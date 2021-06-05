using Interpreter.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Models
{
    public class ObjectInitializationExpressionModel : IExpressionModel
    {
        public ICollection<ObjectPropertyExpressionModel> Properties { get; set; }
    }

    public class ObjectPropertyExpressionModel
    {
        public string Identifier { get; set; }
        public IExpressionModel Value { get; set; }

        public override string ToString() => $"{nameof(ObjectPropertyExpressionModel)}({Identifier}, {Value})";
    }
}
