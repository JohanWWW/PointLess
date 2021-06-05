using Interpreter.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Models
{
    public class AssignObjectPropertyStatementModel : IStatementModel
    {
        public string[] PropertyChain { get; set; }
        public IExpressionModel Assignee { get; set; }
    }
}
