using Interpreter.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Models
{
    public class NativeFunctionStatementModel : IFunctionModel
    {
        public ParameterListModel Parameters { get; set; }
        public Func<IList<dynamic>, dynamic> NativeImplementation { get; set; }

        public NativeFunctionStatementModel()
        {
        }

        public NativeFunctionStatementModel(params string[] parameters)
        {
            Parameters = new ParameterListModel
            {
                Parameters = parameters.ToList()
            };
        }
    }
}
