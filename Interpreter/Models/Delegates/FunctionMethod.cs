using Interpreter.Environment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Models.Delegates
{
    /// <summary>
    /// A method that accepts arguments and returns a value
    /// </summary>
    public delegate dynamic FunctionMethod(dynamic[] args);
}
