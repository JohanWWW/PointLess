using Antlr4.Runtime;
using Interpreter.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Models
{
    public class ThrowStatement : ModelBase, IStatementModel
    {
        public IExpressionModel Expression { get; set; }
        
        public ThrowStatement() : base(Enums.ModelTypeCode.ThrowStatement)
        {
        }
    }
}
