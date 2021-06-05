using Interpreter.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Models
{
    public class ConditionalStatementModel : IStatementModel
    {
        public IfStatementModel If { get; set; }
        public ICollection<ElseIfStatementModel> ElseIf { get; set; }
        public ElseStatementModel Else { get; set; }
    }

    public class IfStatementModel
    {
        public IExpressionModel Condition { get; set; }
        public BlockModel Body { get; set; }
    }

    public class ElseIfStatementModel
    {
        public IExpressionModel Condition { get; set; }
        public BlockModel Body { get; set; }
    }

    public class ElseStatementModel
    {
        public BlockModel Body { get; set; }
    }
}
