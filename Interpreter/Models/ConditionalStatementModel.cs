using Antlr4.Runtime;
using Interpreter.Models.Enums;
using Interpreter.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Models
{
    public class ConditionalStatementModel : ModelBase, IStatementModel
    {
        public IfStatementModel If { get; set; }
        public ICollection<ElseIfStatementModel> ElseIf { get; set; }
        public ElseStatementModel Else { get; set; }
       
        public ConditionalStatementModel() : base(Enums.ModelTypeCode.ConditionalStatement)
        {
        }
    }

    public class IfStatementModel : ModelBase
    {
        public IExpressionModel Condition { get; set; }
        public BlockModel Body { get; set; }

        public IfStatementModel() : base(ModelTypeCode.IfClause)
        {
        }
    }

    public class ElseIfStatementModel : ModelBase
    {
        public IExpressionModel Condition { get; set; }
        public BlockModel Body { get; set; }

        public ElseIfStatementModel() : base(ModelTypeCode.ElseIfClause)
        {
        }
    }

    public class ElseStatementModel : ModelBase
    {
        public BlockModel Body { get; set; }

        public ElseStatementModel() : base(ModelTypeCode.ElseClause)
        {
        }
    }
}
