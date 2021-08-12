using Antlr4.Runtime;
using Interpreter.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Models.Interfaces
{
    public interface IModel
    {
        ModelTypeCode TypeCode { get; }
        IToken StartToken { get; set; }
        IToken StopToken { get; set; }
    }
}
