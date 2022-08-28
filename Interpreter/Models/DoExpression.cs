using Interpreter.Models.Enums;
using Interpreter.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Models
{
    public class DoExpression : ModelBase, IExpressionModel
    {
        public BlockModel Body { get; set; }
        public IExpressionModel Return { get; set; }

        public DoExpression() : base(ModelTypeCode.DoExpression)
        {
        }
    }
}
