using Interpreter.Models.Enums;
using Interpreter.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Models
{
    public class DoWhileLoopStatement : ModelBase, ILoopStatementModel
    {
        public BlockModel Body { get; set; }
        public IExpressionModel Condition { get; set; }

        public DoWhileLoopStatement() : base(ModelTypeCode.DoWhileLoop)
        {
        }
    }
}
