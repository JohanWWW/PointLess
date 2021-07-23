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
    public class AssignStatementModel : IStatementModel
    {
        public string[] Identifier { get; set; }
        public AssignmentOperator OperatorCombination { get; set; }
        public IExpressionModel Assignee { get; set; }
        public IToken StartToken { get; set; }
        public IToken StopToken { get; set; }
    }
}
