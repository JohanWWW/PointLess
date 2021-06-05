using Interpreter.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Models
{
    public class NativeActionStatementModel : IFunctionModel
    {
        public Action NativeImplementation { get; set; }
    }
}
