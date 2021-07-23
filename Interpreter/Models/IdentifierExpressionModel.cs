using Antlr4.Runtime;
using Interpreter.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Models
{
    /// <summary>
    /// Represents a value from the an identifier path
    /// </summary>
    public class IdentifierExpressionModel : IExpressionModel
    {
        /// <summary>
        /// Full path to the identifier
        /// </summary>
        public string[] Identifier { get; set; }
        public IToken StartToken { get; set; }
        public IToken StopToken { get; set; }

        public override string ToString()
        {
            if (Identifier is null || Identifier.Length is 0)
                return "null";

            return Identifier.Length is 1 ? Identifier.Single() : string.Join(".", Identifier);
        }
    }
}
