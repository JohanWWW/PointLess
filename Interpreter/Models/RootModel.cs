using Interpreter.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Models
{
    public class RootModel : ModelBase
    {
        public ICollection<IStatementModel> Statements { get; set; }

        public RootModel() : base(Enums.ModelTypeCode.Root)
        {
        }
    }
}
