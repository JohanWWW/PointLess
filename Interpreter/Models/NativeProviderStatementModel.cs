using Interpreter.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Models
{
    public class NativeProviderStatementModel : IFunctionModel
    {
        public Func<dynamic> NativeImplementation { get; set; }
    }
}
