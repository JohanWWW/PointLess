using Antlr4.Runtime;
using Interpreter.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Models
{
    public class ConsumerStatementModel : ModelBase, IFunctionModel
    {
        public string[] Parameters { get; set; }
        public BlockModel Body { get; set; }

        public ConsumerStatementModel() : base(Enums.ModelTypeCode.ConsumerStatement)
        {
        }
    }
}
