using Interpreter.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Models
{
    public class NameofExpression : ModelBase, IExpressionModel
    {
        public IdentifierExpressionModel IdentifierModel { get; set; }

        public NameofExpression() : base(Enums.ModelTypeCode.NameofExpression)
        {
        }
    }
}
