using Antlr4.Runtime;
using Interpreter.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Models
{
    public class ObjectInitializationExpressionModel : ModelBase, IExpressionModel
    {
        public ICollection<ObjectPropertyExpressionModel> Properties { get; set; }
        
        public ObjectInitializationExpressionModel() : base(Enums.ModelTypeCode.ObjectInitializationExpression)
        {
        }
    }

    public class ObjectPropertyExpressionModel
    {
        public string Identifier { get; set; }
        public IExpressionModel Value { get; set; }

        public override string ToString() => $"{nameof(ObjectPropertyExpressionModel)}({Identifier}, {Value})";
    }
}
