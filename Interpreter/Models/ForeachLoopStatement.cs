using Interpreter.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Models
{
    public class ForeachLoopStatement : ModelBase, ILoopStatementModel
    {
        public string Identifier { get; set; }
        public IExpressionModel EnumerableExpression { get; set; }
        public BlockModel Body { get; set; }

        public ForeachLoopStatement() : base(Enums.ModelTypeCode.ForeachLoopStatement)
        {
        }
    }
}
