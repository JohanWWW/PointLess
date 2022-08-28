using Interpreter.Models.Enums;
using Interpreter.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Models
{
    public class DoStatement : ModelBase, IStatementModel
    {
        public BlockModel Body { get; set; }

        public DoStatement() : base(ModelTypeCode.DoStatement)
        {
        }
    }
}
