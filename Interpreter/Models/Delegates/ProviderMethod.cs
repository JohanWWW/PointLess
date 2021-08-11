using Interpreter.Environment;
using Interpreter.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Interpreter.Models.Delegates
{
    /// <summary>
    /// A method that do not accept arguments but returns a value
    /// </summary>
    public delegate IOperable ProviderMethod();
}
