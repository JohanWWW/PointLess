using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Models.Interfaces
{
    public interface IModel
    {
        IToken StartToken { get; set; }
        IToken StopToken { get; set; }
    }
}
