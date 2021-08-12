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
    public class NativeFunctionStatementModel : ModelBase, IFunctionModel
    {
        public string[] Parameters { get; set; }
        public Func<IList<IOperable>, IOperable> NativeImplementation { get; set; }

        public NativeFunctionStatementModel() : base(Enums.ModelTypeCode.NativeFunctionStatement)
        {
        }

        public NativeFunctionStatementModel(params string[] parameters) : this()
        {
            Parameters = parameters;
        }
    }
}
