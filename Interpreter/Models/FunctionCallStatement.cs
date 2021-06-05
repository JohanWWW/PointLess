using Interpreter.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Models
{
    public class FunctionCallStatement : IExpressionModel, IStatementModel
    {
        public string[] IdentifierPath { get; set; }
        public ArgumentListModel Arguments { get; set; }
    }
}
