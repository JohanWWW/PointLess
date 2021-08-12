using Antlr4.Runtime;
using Interpreter.Models.Interfaces;
using Interpreter.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Models
{
    public class NativeConsumerStatementModel : ModelBase, IFunctionModel
    {
        public string[] Parameters { get; set; }
        public Action<IList<IOperable>> NativeImplementation { get; set; }
        
        public NativeConsumerStatementModel() : base(Enums.ModelTypeCode.NativeConsumerStatement)
        {
        }
    }
}
