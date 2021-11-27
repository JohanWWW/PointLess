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
    public class CompilerConstDefinition : ModelBase, IStatementModel
    {
        public string Identifier { get; set; }
        public IExpressionModel Expression { get; set; }

        public CompilerConstDefinition() : base(ModelTypeCode.CompilerConstDefinition)
        {
        }
    }
}
